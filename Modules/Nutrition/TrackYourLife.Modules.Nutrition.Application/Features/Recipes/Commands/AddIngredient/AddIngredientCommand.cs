using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.AddIngredient;

public sealed record AddIngredientCommand(
    RecipeId RecipeId,
    FoodId FoodId,
    ServingSizeId ServingSizeId,
    float Quantity
) : ICommand<IngredientId>;
