using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.DeleteRecipe;

public sealed class DeleteRecipeCommandHandler(
    IRecipeRepository recipeRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<DeleteRecipeCommand>
{
    public async Task<Result> Handle(
        DeleteRecipeCommand command,
        CancellationToken cancellationToken
    )
    {
        var recipe = await recipeRepository.GetByIdAsync(command.Id, cancellationToken);

        if (recipe is null)
        {
            return Result.Failure(RecipeErrors.NotFound(command.Id));
        }
        else if (recipe.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(RecipeErrors.NotOwned(command.Id));
        }

        recipe.MarkAsOld();

        recipeRepository.Update(recipe);

        return Result.Success();
    }
}
