using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

public static class RecipeErrors
{
    public static Error NotFound(RecipeId id) => Error.NotFound(id, nameof(Recipe));

    public static Error NotOwned(RecipeId id) => Error.NotOwned(id, nameof(Recipe));

    public static Error AlreadyExists(string name) =>
        new("Recipe.AlreadyExists", $"Recipe with name {name} already exists.", 400);

    public static Error Old => new("Recipe.Old", "Recipe is old and cannot be modified.", 400);
}
