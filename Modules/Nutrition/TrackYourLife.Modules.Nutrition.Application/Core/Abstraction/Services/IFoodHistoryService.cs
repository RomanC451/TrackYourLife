using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;

public interface IFoodHistoryService
{
    Task<Result> AddNewFoodAsync(UserId userId, FoodId foodId, CancellationToken cancellationToken);
    Task<IEnumerable<FoodReadModel>> GetFoodHistoryAsync(
        UserId userId,
        CancellationToken cancellationToken
    );
    Task<IEnumerable<FoodReadModel>> PrioritizeHistoryItemsAsync(
        UserId userId,
        IEnumerable<FoodReadModel> searchResults,
        CancellationToken cancellationToken
    );
}
