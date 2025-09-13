using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Contracts.Dtos;

public sealed record RecipeDto(
    RecipeId Id,
    string Name,
    int Portions,
    float Weight,
    List<IngredientDto> Ingredients,
    NutritionalContent NutritionalContents,
    List<ServingSizeDto> ServingSizes
);
