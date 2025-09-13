using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

public sealed record RecipeDiaryReadModel(
    NutritionDiaryId Id,
    UserId UserId,
    ServingSizeId ServingSizeId,
    float Quantity,
    MealTypes MealType,
    DateOnly Date,
    DateTime CreatedOnUtc
) : NutritionDiaryReadModel(Id, UserId, Quantity, MealType, Date, CreatedOnUtc)
{
    public required RecipeReadModel Recipe { get; init; }
}
