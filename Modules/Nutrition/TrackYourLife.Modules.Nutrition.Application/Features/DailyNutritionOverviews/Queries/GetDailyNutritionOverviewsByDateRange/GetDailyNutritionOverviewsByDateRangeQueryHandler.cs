using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;

internal sealed class GetDailyNutritionOverviewsByDateRangeQueryHandler(
    IDailyNutritionOverviewQuery dailyNutritionOverviewQuery,
    IUserIdentifierProvider userIdentifierProvider
)
    : IQueryHandler<
        GetDailyNutritionOverviewsByDateRangeQuery,
        IEnumerable<DailyNutritionOverviewDto>
    >
{
    public async Task<Result<IEnumerable<DailyNutritionOverviewDto>>> Handle(
        GetDailyNutritionOverviewsByDateRangeQuery query,
        CancellationToken cancellationToken
    )
    {
        var overviews = await dailyNutritionOverviewQuery.GetByPeriodAsync(
            userIdentifierProvider.UserId,
            query.StartDate,
            query.EndDate,
            cancellationToken
        );

        if (!overviews.Any())
        {
            return Result.Success(Enumerable.Empty<DailyNutritionOverviewDto>());
        }

        var overviewList = overviews.ToList();
        var dateRange = Enumerable
            .Range(
                0,
                (
                    query.EndDate.ToDateTime(TimeOnly.MinValue)
                    - query.StartDate.ToDateTime(TimeOnly.MinValue)
                ).Days + 1
            )
            .Select(offset => query.StartDate.AddDays(offset))
            .ToList();

        foreach (var date in dateRange)
        {
            var overview = overviewList.Find(o => o.Date == date);
            if (overview == null)
            {
                var closestOverview = overviewList
                    .OrderBy(x =>
                        Math.Abs(
                            (
                                x.Date.ToDateTime(TimeOnly.MinValue)
                                - date.ToDateTime(TimeOnly.MinValue)
                            ).Days
                        )
                    )
                    .FirstOrDefault();

                var newOverview = new DailyNutritionOverviewReadModel(
                    DailyNutritionOverviewId.NewId(),
                    userIdentifierProvider.UserId,
                    date,
                    closestOverview?.CaloriesGoal ?? 0,
                    closestOverview?.CarbohydratesGoal ?? 0,
                    closestOverview?.FatGoal ?? 0,
                    closestOverview?.ProteinGoal ?? 0
                )
                {
                    NutritionalContent = new NutritionalContent(),
                };

                overviewList.Add(newOverview);
            }
        }

        return Result.Success(
            AggregateOverviews(
                overviewList.OrderBy(o => o.Date),
                query.OverviewType,
                query.AggregationMode
            )
        );
    }

    private static IEnumerable<DailyNutritionOverviewDto> AggregateOverviews(
        IEnumerable<DailyNutritionOverviewReadModel> overviews,
        OverviewType overviewType,
        AggregationMode aggregationMode
    )
    {
        return overviewType switch
        {
            OverviewType.Daily => overviews.Select(o => new DailyNutritionOverviewDto(
                o.Id,
                o.Date,
                o.Date,
                o.NutritionalContent,
                o.CaloriesGoal,
                o.CarbohydratesGoal,
                o.FatGoal,
                o.ProteinGoal
            )),
            OverviewType.Weekly => AggregateWeeklyOverviews(overviews, aggregationMode),
            OverviewType.Monthly => AggregateMonthlyOverviews(overviews, aggregationMode),
            _ => overviews.Select(o => new DailyNutritionOverviewDto(
                o.Id,
                o.Date,
                o.Date,
                o.NutritionalContent,
                o.CaloriesGoal,
                o.CarbohydratesGoal,
                o.FatGoal,
                o.ProteinGoal
            )),
        };
    }

    private static IEnumerable<DailyNutritionOverviewDto> AggregateWeeklyOverviews(
        IEnumerable<DailyNutritionOverviewReadModel> overviews,
        AggregationMode aggregationMode
    )
    {
        var weekGroups = overviews
            .GroupBy(o => GetStartOfWeek(o.Date))
            .Select(weekGroup =>
            {
                var orderedOverviews = weekGroup.OrderBy(o => o.Date).ToList();
                var startDate = orderedOverviews[0].Date;
                var endDate = orderedOverviews[^1].Date;
                var count = orderedOverviews.Count;

                var aggregatedNutritionalContent = new NutritionalContent();
                foreach (var overview in orderedOverviews)
                {
                    aggregatedNutritionalContent.AddNutritionalValues(overview.NutritionalContent);
                }

                float caloriesGoal;
                float carbohydratesGoal;
                float fatGoal;
                float proteinGoal;

                if (aggregationMode == AggregationMode.Sum)
                {
                    caloriesGoal = orderedOverviews.Sum(o => o.CaloriesGoal);
                    carbohydratesGoal = orderedOverviews.Sum(o => o.CarbohydratesGoal);
                    fatGoal = orderedOverviews.Sum(o => o.FatGoal);
                    proteinGoal = orderedOverviews.Sum(o => o.ProteinGoal);
                }
                else
                {
                    aggregatedNutritionalContent =
                        aggregatedNutritionalContent.MultiplyNutritionalValues(1f / count);
                    caloriesGoal = orderedOverviews.Average(o => o.CaloriesGoal);
                    carbohydratesGoal = orderedOverviews.Average(o => o.CarbohydratesGoal);
                    fatGoal = orderedOverviews.Average(o => o.FatGoal);
                    proteinGoal = orderedOverviews.Average(o => o.ProteinGoal);
                }

                return new DailyNutritionOverviewDto(
                    DailyNutritionOverviewId.NewId(),
                    startDate,
                    endDate,
                    aggregatedNutritionalContent,
                    caloriesGoal,
                    carbohydratesGoal,
                    fatGoal,
                    proteinGoal
                );
            })
            .OrderBy(dto => dto.StartDate);

        return weekGroups;
    }

    private static IEnumerable<DailyNutritionOverviewDto> AggregateMonthlyOverviews(
        IEnumerable<DailyNutritionOverviewReadModel> overviews,
        AggregationMode aggregationMode
    )
    {
        var monthGroups = overviews
            .GroupBy(o => new { o.Date.Year, o.Date.Month })
            .Select(monthGroup =>
            {
                var orderedOverviews = monthGroup.OrderBy(o => o.Date).ToList();
                var startDate = orderedOverviews[0].Date;
                var endDate = orderedOverviews[^1].Date;
                var count = orderedOverviews.Count;

                var aggregatedNutritionalContent = new NutritionalContent();
                foreach (var overview in orderedOverviews)
                {
                    aggregatedNutritionalContent.AddNutritionalValues(overview.NutritionalContent);
                }

                float caloriesGoal;
                float carbohydratesGoal;
                float fatGoal;
                float proteinGoal;

                if (aggregationMode == AggregationMode.Sum)
                {
                    caloriesGoal = orderedOverviews.Sum(o => o.CaloriesGoal);
                    carbohydratesGoal = orderedOverviews.Sum(o => o.CarbohydratesGoal);
                    fatGoal = orderedOverviews.Sum(o => o.FatGoal);
                    proteinGoal = orderedOverviews.Sum(o => o.ProteinGoal);
                }
                else
                {
                    aggregatedNutritionalContent =
                        aggregatedNutritionalContent.MultiplyNutritionalValues(1f / count);
                    caloriesGoal = orderedOverviews.Average(o => o.CaloriesGoal);
                    carbohydratesGoal = orderedOverviews.Average(o => o.CarbohydratesGoal);
                    fatGoal = orderedOverviews.Average(o => o.FatGoal);
                    proteinGoal = orderedOverviews.Average(o => o.ProteinGoal);
                }

                return new DailyNutritionOverviewDto(
                    DailyNutritionOverviewId.NewId(),
                    startDate,
                    endDate,
                    aggregatedNutritionalContent,
                    caloriesGoal,
                    carbohydratesGoal,
                    fatGoal,
                    proteinGoal
                );
            })
            .OrderBy(dto => dto.StartDate);

        return monthGroups;
    }

    private static DateOnly GetStartOfWeek(DateOnly date)
    {
        var dayOfWeek = date.DayOfWeek;
        var daysToSubtract = dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;
        return date.AddDays(-daysToSubtract);
    }
}
