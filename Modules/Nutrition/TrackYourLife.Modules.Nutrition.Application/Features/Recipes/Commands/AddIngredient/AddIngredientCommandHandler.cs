using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.AddIngredient;

internal sealed class AddIngredientCommandHandler(
    IRecipeRepository recipeRepository,
    IFoodRepository foodRepository,
    IServingSizeQuery servingSizeQuery,
    IUserIdentifierProvider userIdentifierProvider,
    IRecipeService recipeService
) : ICommandHandler<AddIngredientCommand, IngredientId>
{
    public async Task<Result<IngredientId>> Handle(
        AddIngredientCommand command,
        CancellationToken cancellationToken
    )
    {
        var recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);

        if (recipe is null)
            return Result.Failure<IngredientId>(RecipeErrors.NotFound(command.RecipeId));
        else if (recipe.UserId != userIdentifierProvider.UserId)
            return Result.Failure<IngredientId>(RecipeErrors.NotOwned(command.RecipeId));

        var food = await foodRepository.GetByIdAsync(command.FoodId, cancellationToken);

        if (food is null)
            return Result.Failure<IngredientId>(FoodErrors.NotFoundById(command.FoodId));

        if (!food.HasServingSize(command.ServingSizeId))
            return Result.Failure<IngredientId>(
                FoodErrors.ServingSizeNotFound(food.Id, command.ServingSizeId)
            );

        var servingSize = await servingSizeQuery.GetByIdAsync(
            command.ServingSizeId,
            cancellationToken
        );

        if (servingSize is null)
            return Result.Failure<IngredientId>(ServingSizeErrors.NotFound(command.ServingSizeId));

        var ingredientResult = Ingredient.Create(
            userIdentifierProvider.UserId,
            IngredientId.NewId(),
            food.Id,
            command.ServingSizeId,
            command.Quantity
        );

        if (ingredientResult.IsFailure)
            return Result.Failure<IngredientId>(ingredientResult.Error);

        var cloneResult = await recipeService.CloneIfUsed(
            recipe,
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (cloneResult.IsFailure)
            return Result.Failure<IngredientId>(cloneResult.Error);

        var recipeResult = recipe.AddIngredient(ingredientResult.Value, food, servingSize);

        if (recipeResult.IsFailure)
            return Result.Failure<IngredientId>(recipeResult.Error);

        recipeRepository.Update(recipe);

        return Result.Success(ingredientResult.Value.Id);
    }
}
