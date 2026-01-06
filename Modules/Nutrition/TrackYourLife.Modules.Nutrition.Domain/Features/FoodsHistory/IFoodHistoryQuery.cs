using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;

public interface IFoodHistoryQuery
{
    Task<IEnumerable<FoodHistoryReadModel>> GetHistoryByUserAsync(
        UserId userId,
        CancellationToken cancellationToken
    );
    Task<FoodHistoryReadModel?> GetByUserAndFoodAsync(
        UserId userId,
        FoodId foodId,
        CancellationToken cancellationToken
    );
}
