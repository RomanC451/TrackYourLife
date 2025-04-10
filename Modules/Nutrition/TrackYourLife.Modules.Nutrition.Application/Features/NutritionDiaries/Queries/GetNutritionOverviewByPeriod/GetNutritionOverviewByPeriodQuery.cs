using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

namespace TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionOverviewByPeriod;

public sealed record GetNutritionOverviewByPeriodQuery(DateOnly StartDate, DateOnly EndDate)
    : IQuery<NutritionalContent>;
