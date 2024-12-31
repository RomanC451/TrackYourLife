using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

public interface IRecipeQuery
{
    Task<RecipeReadModel?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken);
    Task<IEnumerable<RecipeReadModel>> GetByNameAndUserIdAsync(
        string name,
        UserId userId,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<RecipeReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken
    );
}
