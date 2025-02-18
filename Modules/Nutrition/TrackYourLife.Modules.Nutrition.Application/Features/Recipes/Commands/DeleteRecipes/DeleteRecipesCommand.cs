using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.DeleteRecipes;

public sealed record DeleteRecipesCommand(IEnumerable<RecipeId> Ids) : ICommand;
