using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateIngredient;

public sealed class UpdateIngredientCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IRecipeRepository recipeRepository,
    IFoodRepository foodRepository,
    IServingSizeRepository servingSizeRepository,
    IRecipeManager recipeManager
) : ICommandHandler<UpdateIngredientCommand>
{
    public async Task<Result> Handle(
        UpdateIngredientCommand command,
        CancellationToken cancellationToken
    )
    {
        var recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);

        if (recipe is null)
            return Result.Failure(RecipeErrors.NotFound(command.RecipeId));
        else if (recipe.UserId != userIdentifierProvider.UserId)
            return Result.Failure(RecipeErrors.NotOwned(command.RecipeId));

        var ingredient = recipe.GetIngredientById(command.IngredientId);

        if (ingredient is null)
            return Result.Failure(IngredientErrors.NotFound(command.IngredientId));

        var food = await foodRepository.GetByIdAsync(ingredient.FoodId, cancellationToken);

        if (food is null)
            return Result.Failure(FoodErrors.NotFoundById(ingredient.FoodId));

        var oldServingSize = await servingSizeRepository.GetByIdAsync(
            ingredient.ServingSizeId,
            cancellationToken
        );

        if (oldServingSize is null)
            return Result.Failure(ServingSizeErrors.NotFound(ingredient.ServingSizeId));

        var newServingSize = await servingSizeRepository.GetByIdAsync(
            command.ServingSizeId,
            cancellationToken
        );

        if (newServingSize is null)
            return Result.Failure(ServingSizeErrors.NotFound(command.ServingSizeId));

        if (!food.HasServingSize(newServingSize.Id))
            return Result.Failure(FoodErrors.ServingSizeNotFound(food.Id, command.ServingSizeId));

        var cloneResult = await recipeManager.CloneIfUsed(recipe, cancellationToken);

        if (cloneResult.IsFailure)
            return Result.Failure(cloneResult.Error);

        ingredient = recipe.GetIngredientByFoodId(ingredient.FoodId);

        if (ingredient is null)
            return Result.Failure(IngredientErrors.NotFound(command.IngredientId));

        var oldQuantity = ingredient.Quantity;

        var result = Result.FirstFailureOrSuccess(
            ingredient.UpdateQuantity(command.Quantity),
            ingredient.UpdateServingSize(newServingSize.Id)
        );

        recipe.UpdateIngredient(food, ingredient, oldServingSize, newServingSize, oldQuantity);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        recipeRepository.Update(recipe);

        return Result.Success();
    }
}
