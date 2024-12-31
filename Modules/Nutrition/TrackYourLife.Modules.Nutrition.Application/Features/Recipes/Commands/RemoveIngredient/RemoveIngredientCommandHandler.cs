using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.RemoveIngredient;

public sealed class RemoveIngredientCommandHandler(
    IRecipeRepository recipeRepository,
    IFoodRepository foodRepository,
    IServingSizeQuery servingSizeQuery,
    IUserIdentifierProvider userIdentifierProvider,
    IRecipeManager recipeManager
) : ICommandHandler<RemoveIngredientCommand>
{
    public async Task<Result> Handle(
        RemoveIngredientCommand command,
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

        var servingSize = await servingSizeQuery.GetByIdAsync(
            ingredient.ServingSizeId,
            cancellationToken
        );

        if (servingSize is null)
            return Result.Failure(ServingSizeErrors.NotFound(ingredient.ServingSizeId));

        var cloneResult = await recipeManager.CloneIfUsed(recipe, cancellationToken);

        if (cloneResult.IsFailure)
            return Result.Failure(cloneResult.Error);

        recipe.RemoveIngredient(ingredient, food, servingSize);

        recipeRepository.Update(recipe);

        return Result.Success();
    }
}
