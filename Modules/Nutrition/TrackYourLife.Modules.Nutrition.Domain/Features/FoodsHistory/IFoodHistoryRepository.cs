using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;

public interface IFoodHistoryRepository
{
    Task<FoodHistory?> GetByUserAndFoodAsync(
        UserId userId,
        FoodId foodId,
        CancellationToken cancellationToken
    );
    Task AddAsync(FoodHistory foodHistory, CancellationToken cancellationToken);
    void Update(FoodHistory foodHistory);
    void Remove(FoodHistory foodHistory);
    Task<int> GetUserHistoryCountAsync(UserId userId, CancellationToken cancellationToken);
    Task<FoodHistory?> GetOldestByUserAsync(UserId userId, CancellationToken cancellationToken);
}
