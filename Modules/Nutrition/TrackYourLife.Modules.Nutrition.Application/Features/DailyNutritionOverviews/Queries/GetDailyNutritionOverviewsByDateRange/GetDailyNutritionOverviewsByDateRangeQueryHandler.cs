using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;

public sealed class GetDailyNutritionOverviewsByDateRangeQueryHandler(
    IDailyNutritionOverviewQuery dailyNutritionOverviewQuery,
    IUserIdentifierProvider userIdentifierProvider
)
    : IQueryHandler<
        GetDailyNutritionOverviewsByDateRangeQuery,
        IEnumerable<DailyNutritionOverviewReadModel>
    >
{
    public async Task<Result<IEnumerable<DailyNutritionOverviewReadModel>>> Handle(
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
            return Result.Success(Enumerable.Empty<DailyNutritionOverviewReadModel>());
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

        return Result.Success(overviewList.OrderBy(o => o.Date).AsEnumerable());
    }
}
