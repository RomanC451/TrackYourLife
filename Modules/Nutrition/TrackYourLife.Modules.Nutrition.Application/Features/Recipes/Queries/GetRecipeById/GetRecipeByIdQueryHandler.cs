using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipeById;

internal sealed class GetRecipeByIdQueryHandler(
    IRecipeQuery recipeQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetRecipeByIdQuery, RecipeReadModel>
{
    public async Task<Result<RecipeReadModel>> Handle(
        GetRecipeByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var recipe = await recipeQuery.GetByIdAsync(query.Id, cancellationToken);

        if (recipe is null)
        {
            return Result.Failure<RecipeReadModel>(RecipeErrors.NotFound(query.Id));
        }

        if (recipe.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure<RecipeReadModel>(RecipeErrors.NotOwned(query.Id));
        }

        return Result.Create(recipe);
    }
}
