using System.Collections.ObjectModel;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipesByUserId;

internal sealed class GetRecipesByUserIdQueryHandler(
    IRecipeQuery recipeQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetRecipesByUserIdQuery, ReadOnlyCollection<RecipeReadModel>>
{
    public async Task<Result<ReadOnlyCollection<RecipeReadModel>>> Handle(
        GetRecipesByUserIdQuery query,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<RecipeReadModel> recipes;

        recipes = await recipeQuery.GetByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        return Result.Create(recipes.ToList().AsReadOnly());
    }
}
