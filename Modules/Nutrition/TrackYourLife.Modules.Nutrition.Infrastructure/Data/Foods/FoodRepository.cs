using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Foods;

internal sealed class FoodRepository(NutritionWriteDbContext context)
    : GenericRepository<Food, FoodId>(context.Foods, CreateQuery(context.Foods)),
        IFoodRepository
{
    private static IQueryable<Food> CreateQuery(DbSet<Food> dbSet)
    {
        return dbSet.Include(f => f.FoodServingSizes);
    }

    public async Task<Food?> GetByApiIdAsync(long apiId, CancellationToken cancellationToken)
    {
        return await query.FirstOrDefaultAsync(f => f.ApiId == apiId, cancellationToken);
    }

    public async Task<List<Food>> GetWhereApiIdPartOfListAsync(
        List<long> apiIds,
        CancellationToken cancellationToken
    )
    {
        return await query
            .Where(f => f.ApiId.HasValue && apiIds.Contains(f.ApiId.Value))
            .ToListAsync(cancellationToken);
    }
}
