using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Events;

public class FoodDiaryCreatedDomainEventHandler(IFoodHistoryService foodHistoryService)
    : IDomainEventHandler<FoodDiaryCreatedDomainEvent>
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
    }
}
