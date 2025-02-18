using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;

public sealed class FoodDiaryDeletedDomainEventHandler(
    IFoodQuery foodQuery,
    IServingSizeQuery servingSizeQuery,
    IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
    INutritionUnitOfWork nutritionUnitOfWork
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

        if (food is null || servingSize is null)
        {
            return;
        }

        var dailyNutritionOverview = await dailyNutritionOverviewRepository.GetByUserIdAndDateAsync(
            notification.UserId,
            notification.Date,
            cancellationToken
        );

        if (dailyNutritionOverview is null)
        {
            return;
        }

        dailyNutritionOverview.SubtractNutritionalValues(
            food.NutritionalContents,
            notification.Quantity * servingSize.NutritionMultiplier
        );

        dailyNutritionOverviewRepository.Update(dailyNutritionOverview);

        await nutritionUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
