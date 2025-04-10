using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;

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
            cancellationToken
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
