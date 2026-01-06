using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodsHistory;

public interface IFoodHistoryService
{
    Task<Result> AddNewFoodAsync(
        UserId userId,
        FoodId foodId,
        ServingSizeId servingSizeId,
        float quantity,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<FoodDto>> GetFoodHistoryAsync(
        UserId userId,
        CancellationToken cancellationToken
    );
    Task<IEnumerable<FoodDto>> PrioritizeHistoryItemsAsync(
        UserId userId,
        IEnumerable<FoodReadModel> searchResults,
        CancellationToken cancellationToken
    );
}
