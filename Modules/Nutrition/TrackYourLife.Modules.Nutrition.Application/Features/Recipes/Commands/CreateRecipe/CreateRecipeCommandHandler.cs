using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.CreateRecipe;

internal sealed class CreateRecipeCommandHandler(
    IRecipeRepository recipeRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<CreateRecipeCommand, RecipeId>
{
    public async Task<Result<RecipeId>> Handle(
        CreateRecipeCommand command,
        CancellationToken cancellationToken
    )
    {
        // Check if a recipe with the same name already exists for this user
        var existingRecipe = await recipeRepository.GetByNameAndUserIdAsync(
            command.Name,
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (existingRecipe is not null)
        {
            return Result.Failure<RecipeId>(RecipeErrors.AlreadyExists(command.Name));
        }

        var result = Recipe.Create(
            RecipeId.NewId(),
            userIdentifierProvider.UserId,
            command.Name,
            command.Weight,
            command.Portions
        );

        if (result.IsFailure)
        {
            return Result.Failure<RecipeId>(result.Error);
        }

        await recipeRepository.AddAsync(result.Value, cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
