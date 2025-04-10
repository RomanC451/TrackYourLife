using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UndoDeleteRecipe;

internal sealed class UndoDeleteRecipeCommandHandler(
    IRecipeRepository recipeRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UndoDeleteRecipeCommand>
{
    public async Task<Result> Handle(
        UndoDeleteRecipeCommand command,
        CancellationToken cancellationToken
    )
    {
        var recipe = await recipeRepository.GetOldByIdAsync(command.Id, cancellationToken);

        if (recipe is null)
        {
            return Result.Failure(RecipeErrors.NotFound(command.Id));
        }
        else if (recipe.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(RecipeErrors.NotOwned(command.Id));
        }

        recipe.RemoveOldMark();

        recipeRepository.Update(recipe);

        return Result.Success();
    }
}
