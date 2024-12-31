using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;

public interface IFoodHistoryQuery
{
    Task<IEnumerable<FoodHistoryReadModel>> GetHistoryByUserAsync(
        UserId userId,
        CancellationToken cancellationToken
    );
}
