using Serilog;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.OutboxMessages;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;

internal sealed class FoodDiaryDeletedDomainEventHandler(
    IFoodQuery foodQuery,
    IServingSizeQuery servingSizeQuery,
    IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
    INutritionUnitOfWork nutritionUnitOfWork,
    ILogger logger
) : IDomainEventHandler<FoodDiaryDeletedDomainEvent>
{
    public async Task Handle(
        FoodDiaryDeletedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var food = await foodQuery.GetByIdAsync(notification.FoodId, cancellationToken);

        var servingSize = await servingSizeQuery.GetByIdAsync(
            notification.ServingSizeId,
            cancellationToken
        );

        if (food is null)
        {
            logger.Error(
                "Failed to handle FoodDiaryDeletedDomainEvent: Food with id {FoodId} not found",
                notification.FoodId
            );
            throw new EventFailedException(
                $"Food with id {notification.FoodId} not found when handling FoodDiaryDeletedDomainEvent"
            );
        }

        if (servingSize is null)
        {
            logger.Error(
                "Failed to handle FoodDiaryDeletedDomainEvent: ServingSize with id {ServingSizeId} not found",
                notification.ServingSizeId
            );
            throw new EventFailedException(
                $"ServingSize with id {notification.ServingSizeId} not found when handling FoodDiaryDeletedDomainEvent"
            );
        }

        var dailyNutritionOverview = await dailyNutritionOverviewRepository.GetByUserIdAndDateAsync(
            notification.UserId,
            notification.Date,
            cancellationToken
        );

        if (dailyNutritionOverview is null)
        {
            logger.Error(
                "Failed to handle FoodDiaryDeletedDomainEvent: DailyNutritionOverview not found for UserId={UserId}, Date={Date}",
                notification.UserId,
                notification.Date
            );
            throw new EventFailedException(
                $"DailyNutritionOverview not found for UserId {notification.UserId} and Date {notification.Date} when handling FoodDiaryDeletedDomainEvent"
            );
        }

        dailyNutritionOverview.SubtractNutritionalValues(
            food.NutritionalContents,
            notification.Quantity * servingSize.NutritionMultiplier
        );

        dailyNutritionOverviewRepository.Update(dailyNutritionOverview);

        await nutritionUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
