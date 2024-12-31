namespace TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetTotalCaloriesByPeriod;

public sealed record GetTotalCaloriesByPeriodQuery(DateOnly StartDate, DateOnly EndDate)
    : IQuery<int>;
