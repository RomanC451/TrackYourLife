using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;

public sealed record GetDailyNutritionOverviewsByDateRangeQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    OverviewType OverviewType,
    AggregationMode AggregationMode
) : IQuery<IEnumerable<DailyNutritionOverviewDto>>;
