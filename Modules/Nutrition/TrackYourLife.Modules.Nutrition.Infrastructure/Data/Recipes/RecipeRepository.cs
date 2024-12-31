using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Recipes.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Recipes;

public class RecipeRepository(NutritionWriteDbContext context)
    : GenericRepository<Recipe, RecipeId>(context.Recipes, CreateQuery(context.Recipes)),
        IRecipeRepository
{
    private static IQueryable<Recipe> CreateQuery(DbSet<Recipe> dbSet)
    {
        return dbSet.Include(r => r.Ingredients).Where(r => !r.IsOld);
    }

    public Task<Recipe?> GetByNameAndUserIdAsync(
        string name,
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        return FirstOrDefaultAsync(
            new RecipeWithNameAndUserIdSpecification(name, userId),
            cancellationToken
        );
    }
}
