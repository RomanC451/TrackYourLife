using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipeName;

public sealed class UpdateRecipeNameCommandHandler(
    IRecipeRepository recipeRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateRecipeNameCommand>
{
    public async Task<Result> Handle(
        UpdateRecipeNameCommand command,
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

        if (existingRecipe is not null)
        {
            return Result.Failure(RecipeErrors.AlreadyExists(command.Name));
        }

        recipe.UpdateName(command.Name);

        recipeRepository.Update(recipe);

        return Result.Success();
    }
}
