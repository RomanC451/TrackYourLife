using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Contracts.Dtos;

public sealed record RecipeDiaryDto(
    NutritionDiaryId Id,
    RecipeDto Recipe,
    MealTypes MealType,
    float Quantity,
    DateOnly Date
);
