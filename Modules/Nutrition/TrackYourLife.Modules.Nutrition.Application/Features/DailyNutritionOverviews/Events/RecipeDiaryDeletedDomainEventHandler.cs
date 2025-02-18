using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;

public class RecipeDiaryDeletedDomainEventHandler(
    IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
    IQueryRepository queryRepository,
    INutritionUnitOfWork nutritionUnitOfWork
) : IDomainEventHandler<RecipeDiaryDeletedDomainEvent>
{
    public async Task Handle(
        RecipeDiaryDeletedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var recipe = await queryRepository.GetByIdAsync(notification.RecipeId, cancellationToken);

        if (recipe is null)
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
            recipe.NutritionalContents,
            notification.Quantity
        );

        dailyNutritionOverviewRepository.Update(dailyNutritionOverview);

        await nutritionUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
