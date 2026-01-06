using Serilog;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.OutboxMessages;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;

internal sealed class RecipeDiaryDeletedDomainEventHandler(
    IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
    IRecipeRepository queryRepository,
    INutritionUnitOfWork nutritionUnitOfWork,
    ILogger logger
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
            logger.Error(
                "Failed to handle RecipeDiaryDeletedDomainEvent: Recipe with id {RecipeId} not found",
                notification.RecipeId
            );
            throw new EventFailedException(
                $"Recipe with id {notification.RecipeId} not found when handling RecipeDiaryDeletedDomainEvent"
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
                "Failed to handle RecipeDiaryDeletedDomainEvent: DailyNutritionOverview not found for UserId={UserId}, Date={Date}",
                notification.UserId,
                notification.Date
            );
            throw new EventFailedException(
                $"DailyNutritionOverview not found for UserId {notification.UserId} and Date {notification.Date} when handling RecipeDiaryDeletedDomainEvent"
            );
        }

        var servingSize = recipe.ServingSizes.FirstOrDefault(x =>
            x.Id == notification.ServingSizeId
        );

        if (servingSize is null)
        {
            logger.Error(
                "Failed to handle RecipeDiaryDeletedDomainEvent: ServingSize with id {ServingSizeId} not found in Recipe {RecipeId}",
                notification.ServingSizeId,
                notification.RecipeId
            );
            throw new EventFailedException(
                $"ServingSize with id {notification.ServingSizeId} not found in Recipe {notification.RecipeId} when handling RecipeDiaryDeletedDomainEvent"
            );
        }

        dailyNutritionOverview.SubtractNutritionalValues(
            recipe.NutritionalContents,
            servingSize.NutritionMultiplier * notification.Quantity
        );

        dailyNutritionOverviewRepository.Update(dailyNutritionOverview);

        await nutritionUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
