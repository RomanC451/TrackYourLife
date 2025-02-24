using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients.Events;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodsHistory.Events
{
    public class IngredientCreatedDomainEventHandler(
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
                cancellationToken
            );

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
