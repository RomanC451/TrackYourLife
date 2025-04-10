using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipe;

public sealed record UpdateRecipeCommand(RecipeId RecipeId, string Name, int Portions) : ICommand;
