using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

namespace TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetTotalCaloriesByPeriod;

public sealed record GetNutritionTotalsByPeriodQuery(DateOnly StartDate, DateOnly EndDate)
    : IQuery<NutritionalContent>;
