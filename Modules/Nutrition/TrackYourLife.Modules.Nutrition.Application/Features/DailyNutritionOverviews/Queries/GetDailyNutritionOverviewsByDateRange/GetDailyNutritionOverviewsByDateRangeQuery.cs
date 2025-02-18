using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;

public sealed record GetDailyNutritionOverviewsByDateRangeQuery(
    DateOnly StartDate,
    DateOnly EndDate
) : IQuery<IEnumerable<DailyNutritionOverviewReadModel>>;
