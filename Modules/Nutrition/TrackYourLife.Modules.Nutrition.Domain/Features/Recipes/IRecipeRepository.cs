using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken);

    Task<Recipe?> GetByNameAndUserIdAsync(
        string name,
        UserId userId,
        CancellationToken cancellationToken
    );
    Task AddAsync(Recipe recipe, CancellationToken cancellationToken);

    void Update(Recipe recipe);
}
