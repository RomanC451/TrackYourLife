using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Contracts.Dtos;

public sealed record FoodDiaryDto(
    NutritionDiaryId Id,
    FoodDto Food,
    MealTypes MealType,
    float Quantity,
    ServingSizeDto ServingSize,
    DateOnly Date
);
