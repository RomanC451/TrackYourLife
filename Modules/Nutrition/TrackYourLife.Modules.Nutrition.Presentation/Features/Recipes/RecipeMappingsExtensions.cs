using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Foods;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes;

/// <summary>
/// Represents the extension class for mapping between different types related to recipes.
/// </summary>

internal static class RecipeMappingsExtensions
{
    public static IngredientDto ToDto(this IngredientReadModel ingredient)
    {
        return new IngredientDto(
            ingredient.Id,
            ingredient.Food.ToDto(),
            ingredient.ServingSize.ToDto(),
            ingredient.Quantity
        );
    }

    public static RecipeDto ToDto(this RecipeReadModel recipe)
    {
        return new RecipeDto(
            recipe.Id,
            recipe.Name,
            recipe.Portions,
            recipe.Ingredients.Select(i => i.ToDto()).ToList(),
            recipe.NutritionalContents
        );
    }
}
