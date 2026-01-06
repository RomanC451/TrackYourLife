using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodsHistory.Events;

internal sealed class FoodDiaryCreatedDomainEventHandler(
    IFoodHistoryService foodHistoryService,
    INutritionUnitOfWork unitOfWork
) : IDomainEventHandler<FoodDiaryCreatedDomainEvent>
{
    public async Task Handle(
        FoodDiaryCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        await foodHistoryService.AddNewFoodAsync(
            notification.UserId,
            notification.FoodId,
            notification.ServingSizeId,
            notification.Quantity,
            cancellationToken
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
