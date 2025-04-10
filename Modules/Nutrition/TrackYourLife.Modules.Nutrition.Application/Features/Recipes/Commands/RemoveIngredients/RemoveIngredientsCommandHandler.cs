using Serilog;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.RemoveIngredients;

internal sealed class RemoveIngredientsCommandHandler(
    IRecipeRepository recipeRepository,
    IFoodRepository foodRepository,
    IServingSizeQuery servingSizeQuery,
    IUserIdentifierProvider userIdentifierProvider,
    IRecipeService recipeService,
    ILogger logger
) : ICommandHandler<RemoveIngredientsCommand>
{
    public async Task<Result> Handle(
        RemoveIngredientsCommand command,
        CancellationToken cancellationToken
    )
    {
        var recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);

        if (recipe is null)
            return Result.Failure(RecipeErrors.NotFound(command.RecipeId));
        else if (recipe.UserId != userIdentifierProvider.UserId)
            return Result.Failure(RecipeErrors.NotOwned(command.RecipeId));

        var cloneResult = await recipeService.CloneIfUsed(
            recipe,
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (cloneResult.IsFailure)
            return Result.Failure(cloneResult.Error);

        foreach (var ingredientId in command.IngredientsIds)
            await RemoveIngredient(recipe, ingredientId, cancellationToken);

        recipeRepository.Update(recipe);

        return Result.Success();
    }

    private async Task RemoveIngredient(
        Recipe recipe,
        IngredientId ingredientId,
        CancellationToken cancellationToken
    )
    {
        var ingredient = recipe.GetIngredientById(ingredientId);

        if (ingredient is null)
        {
            logger.Error(
                "Ingredient with id {IngredientId} not found in recipe with id {RecipeId}",
                ingredientId,
                recipe.Id
            );
            return;
        }

        var food = await foodRepository.GetByIdAsync(ingredient.FoodId, cancellationToken);

        if (food is null)
        {
            logger.Error(
                "Food with id {FoodId} not found, when trying to remove ingredient with id {IngredientId}",
                ingredient.FoodId,
                ingredient.Id
            );
            return;
        }

        var servingSize = await servingSizeQuery.GetByIdAsync(
            ingredient.ServingSizeId,
            cancellationToken
        );

        if (servingSize is null)
        {
            logger.Error(
                "Serving size with id {ServingSizeId} not found, when trying to remove ingredient with id {IngredientId}",
                ingredient.ServingSizeId,
                ingredient.Id
            );
            return;
        }

        recipe.RemoveIngredient(ingredient, food, servingSize);
    }
}
