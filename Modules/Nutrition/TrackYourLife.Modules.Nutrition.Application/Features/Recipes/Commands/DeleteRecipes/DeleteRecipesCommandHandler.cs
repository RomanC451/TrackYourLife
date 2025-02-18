using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.DeleteRecipes;

public sealed class DeleteRecipesCommandHandler(
    IQueryRepository recipeRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<DeleteRecipesCommand>
{
    public async Task<Result> Handle(
        DeleteRecipesCommand command,
        CancellationToken cancellationToken
    )
    {
        var recipes = new List<Recipe>();

        foreach (var id in command.Ids)
        {
            var recipe = await recipeRepository.GetByIdAsync(id, cancellationToken);

            if (recipe is null)
            {
                return Result.Failure(RecipeErrors.NotFound(id));
            }
            else if (recipe.UserId != userIdentifierProvider.UserId)
            {
                return Result.Failure(RecipeErrors.NotOwned(id));
            }

            recipes.Add(recipe);
        }

        foreach (var recipe in recipes)
        {
            recipe.MarkAsOld();
            recipeRepository.Update(recipe);
        }

        return Result.Success();
    }
}
