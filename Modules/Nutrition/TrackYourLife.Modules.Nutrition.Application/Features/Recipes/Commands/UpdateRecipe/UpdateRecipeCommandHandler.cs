using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipe;

internal sealed class UpdateRecipeCommandHandler(
    IRecipeRepository recipeRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateRecipeCommand>
{
    public async Task<Result> Handle(
        UpdateRecipeCommand command,
        CancellationToken cancellationToken
    )
    {
        var recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);

        if (recipe is null)
        {
            return Result.Failure(RecipeErrors.NotFound(command.RecipeId));
        }
        else if (recipe.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(RecipeErrors.NotOwned(command.RecipeId));
        }

        // Check if a recipe with the same name already exists for this user (excluding current recipe)
        var existingRecipe = await recipeRepository.GetByNameAndUserIdAsync(
            command.Name,
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (existingRecipe is not null && existingRecipe.Id != recipe.Id)
        {
            return Result.Failure(RecipeErrors.AlreadyExists(command.Name));
        }

        var result = Result.FirstFailureOrSuccess(
            recipe.UpdateName(command.Name),
            recipe.UpdatePortions(command.Portions),
            recipe.UpdateWeight(command.Weight)
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        recipeRepository.Update(recipe);

        return Result.Success();
    }
}
