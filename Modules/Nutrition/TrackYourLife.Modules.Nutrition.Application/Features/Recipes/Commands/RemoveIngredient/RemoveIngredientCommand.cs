using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.RemoveIngredient;

public sealed record RemoveIngredientCommand(RecipeId RecipeId, IngredientId IngredientId)
    : ICommand;
