using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodsHistory.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodsHistory;

internal sealed class FoodHistoryRepository(NutritionWriteDbContext context)
    : GenericRepository<FoodHistory, FoodHistoryId>(context.FoodHistories),
        IFoodHistoryRepository
{
    public async Task<FoodHistory?> GetByUserAndFoodAsync(
        UserId userId,
        FoodId foodId,
        CancellationToken cancellationToken
    )
    {
        return await FirstOrDefaultAsync(
            new FoodHistoryWithUserIdAndFoodIdSpecification(userId, foodId),
            cancellationToken
        );
    }

    public async Task<int> GetUserHistoryCountAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        return await query.CountAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<FoodHistory?> GetOldestByUserAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        return await query
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.LastUsedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
