using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.DeleteRecipe;

public sealed record DeleteRecipeCommand(RecipeId Id) : ICommand;
