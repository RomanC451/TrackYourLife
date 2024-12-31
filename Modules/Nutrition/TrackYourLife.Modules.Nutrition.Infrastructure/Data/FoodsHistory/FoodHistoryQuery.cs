using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodsHistory.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodsHistory;

internal sealed class FoodHistoryQuery(NutritionReadDbContext context)
    : GenericQuery<FoodHistoryReadModel, FoodHistoryId>(context.FoodHistories),
        IFoodHistoryQuery
{
    public async Task<IEnumerable<FoodHistoryReadModel>> GetHistoryByUserAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        return (
            await WhereAsync(
                new FoodHistoryReadModelWithUserIdSpecification(userId),
                cancellationToken
            )
        )
            .OrderByDescending(x => x.LastUsedAt)
            .ToList();
    }
}
