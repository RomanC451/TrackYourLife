using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

namespace TrackYourLife.Modules.Nutrition.Contracts.Dtos;

public sealed record DailyNutritionOverviewDto(
    DailyNutritionOverviewId Id,
    DateOnly StartDate,
    DateOnly EndDate,
    NutritionalContent NutritionalContent,
    float CaloriesGoal,
    float CarbohydratesGoal,
    float FatGoal,
    float ProteinGoal
);
