using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateIngredient;

public sealed record UpdateIngredientCommand(
    RecipeId RecipeId,
    IngredientId IngredientId,
    ServingSizeId ServingSizeId,
    float Quantity
) : ICommand;
