using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UndoDeleteRecipe;

public sealed record UndoDeleteRecipeCommand(RecipeId Id) : ICommand;
