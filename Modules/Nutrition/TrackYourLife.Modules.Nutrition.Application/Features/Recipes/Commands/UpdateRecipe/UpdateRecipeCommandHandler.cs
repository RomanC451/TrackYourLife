using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

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

        var existingRecipe = await recipeRepository.GetByNameAndUserIdAsync(
            command.Name,
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (existingRecipe is not null && existingRecipe.Id != command.RecipeId)
        {
            return Result.Failure(RecipeErrors.AlreadyExists(command.Name));
        }

        var result = Result.FirstFailureOrSuccess(
            recipe.UpdateName(command.Name),
            recipe.UpdatePortions(command.Portions)
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        recipeRepository.Update(recipe);

        return Result.Success();
    }
}
