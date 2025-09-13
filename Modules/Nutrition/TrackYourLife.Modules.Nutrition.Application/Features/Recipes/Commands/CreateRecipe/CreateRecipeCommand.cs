using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.CreateRecipe;

public sealed record CreateRecipeCommand(string Name, int Portions, float Weight)
    : ICommand<RecipeId>;
