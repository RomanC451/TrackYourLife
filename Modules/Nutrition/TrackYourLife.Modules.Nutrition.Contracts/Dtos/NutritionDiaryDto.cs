using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

namespace TrackYourLife.Modules.Nutrition.Contracts.Dtos;

public sealed record NutritionDiaryDto(
    NutritionDiaryId Id,
    string Name,
    ServingSizeDto? ServingSize,
    NutritionalContent NutritionalContents,
    float NutritionMultiplier,
    float Quantity,
    MealTypes MealType,
    DiaryType DiaryType,
    DateOnly Date
);
