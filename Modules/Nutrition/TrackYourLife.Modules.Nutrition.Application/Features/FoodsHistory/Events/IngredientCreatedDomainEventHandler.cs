using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients.Events;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodsHistory.Events;

internal sealed class IngredientCreatedDomainEventHandler(
    IFoodHistoryService foodHistoryService,
    INutritionUnitOfWork unitOfWork
) : IDomainEventHandler<IngredientCreatedDomainEvent>
{
    public async Task Handle(
        IngredientCreatedDomainEvent notification,
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
