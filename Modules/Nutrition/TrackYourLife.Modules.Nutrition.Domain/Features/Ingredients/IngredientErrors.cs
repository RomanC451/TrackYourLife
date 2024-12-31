using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;

public static class IngredientErrors
{
    public static readonly Func<IngredientId, Error> NotFound = id =>
        Error.NotFound(id, nameof(Ingredient));
}
