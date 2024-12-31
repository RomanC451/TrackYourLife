using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Recipes.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Recipes;

internal class RecipeQuery(NutritionReadDbContext context)
    : GenericQuery<RecipeReadModel, RecipeId>(CreateQuery(context.Recipes)),
        IRecipeQuery
{
    private static IQueryable<RecipeReadModel> CreateQuery(DbSet<RecipeReadModel> dbSet)
    {
        return dbSet
            .Include(r => r.Ingredients.OrderBy(i => i.CreatedOnUtc))
            .ThenInclude(i => i.ServingSize)
            .Include(r => r.Ingredients.OrderBy(i => i.CreatedOnUtc))
            .ThenInclude(i => i.Food)
            .ThenInclude(f => f.FoodServingSizes)
            .ThenInclude(fss => fss.ServingSize)
            .Where(r => !r.IsOld);
    }

    public async Task<IEnumerable<RecipeReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        return await WhereAsync(new RecipeReadModelUserIdSpecification(userId), cancellationToken);
    }

    public Task<IEnumerable<RecipeReadModel>> GetByNameAndUserIdAsync(
        string name,
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        return WhereAsync(
            new RecipeReadModelUserIdAndContainingNameSpecification(name, userId),
            cancellationToken
        );
    }
}
