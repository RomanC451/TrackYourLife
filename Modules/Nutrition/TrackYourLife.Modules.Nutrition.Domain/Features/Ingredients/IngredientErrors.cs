using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;

public static class IngredientErrors
{
    public static readonly Func<IngredientId, Error> NotFound = id =>
        Error.NotFound(id, nameof(Ingredient));

    public static readonly Func<FoodId, Error> DifferentServingSize = foodId => new Error(
        "Ingredient.DifferentServingSize",
        $"Food already added to the recipe but with a different serving size.",
        400
    );
}
