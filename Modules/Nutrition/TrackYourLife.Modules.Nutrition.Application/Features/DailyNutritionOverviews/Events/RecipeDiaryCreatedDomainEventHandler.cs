using MassTransit;
using Serilog;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.OutboxMessages;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;

internal sealed class RecipeDiaryCreatedDomainEventHandler(
    IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
    IRecipeQuery recipeQuery,
    INutritionUnitOfWork nutritionUnitOfWork,
    IBus bus,
    ILogger logger
) : IDomainEventHandler<RecipeDiaryCreatedDomainEvent>
{
    public async Task Handle(
        RecipeDiaryCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var recipe = await recipeQuery.GetByIdAsync(notification.RecipeId, cancellationToken);

        if (recipe is null)
        {
            logger.Error(
                "Failed to handle RecipeDiaryCreatedDomainEvent: Recipe with id {RecipeId} not found",
                notification.RecipeId
            );
            throw new EventFailedException(
                $"Recipe with id {notification.RecipeId} not found when handling RecipeDiaryCreatedDomainEvent"
            );
        }

        var dailyNutritionOverview = await dailyNutritionOverviewRepository.GetByUserIdAndDateAsync(
            notification.UserId,
            notification.Date,
            cancellationToken
        );

        var servingSize = recipe.ServingSizes.FirstOrDefault(x =>
            x.Id == notification.ServingSizeId
        );

        if (servingSize is null)
        {
            logger.Error(
                "Failed to handle RecipeDiaryCreatedDomainEvent: ServingSize with id {ServingSizeId} not found in Recipe {RecipeId}",
                notification.ServingSizeId,
                notification.RecipeId
            );
            throw new EventFailedException(
                $"ServingSize with id {notification.ServingSizeId} not found in Recipe {notification.RecipeId} when handling RecipeDiaryCreatedDomainEvent"
            );
        }

        if (dailyNutritionOverview is null)
        {
            var client = bus.CreateRequestClient<GetNutritionGoalsByUserIdRequest>();

            var response = await client.GetResponse<GetNutritionGoalsByUserIdResponse>(
                new GetNutritionGoalsByUserIdRequest(notification.UserId, notification.Date),
                cancellationToken
            );

            if (response.Message.Data is null)
            {
                logger.Error(
                    "Failed to handle RecipeDiaryCreatedDomainEvent: Failed to get nutrition goals for UserId={UserId}, Date={Date}. Errors: {Errors}",
                    notification.UserId,
                    notification.Date,
                    response.Message.Errors
                );
                throw new EventFailedException(
                    $"Failed to get nutrition goals for UserId {notification.UserId} and Date {notification.Date} when handling RecipeDiaryCreatedDomainEvent. Errors: {string.Join(", ", response.Message.Errors)}"
                );
            }
            else
            {
                var result = DailyNutritionOverview.Create(
                    DailyNutritionOverviewId.NewId(),
                    notification.UserId,
                    notification.Date,
                    response.Message.Data.CaloriesGoal,
                    response.Message.Data.CarbohydratesGoal,
                    response.Message.Data.FatGoal,
                    response.Message.Data.ProteinGoal
                );

                if (result.IsFailure)
                {
                    logger.Error(
                        "Failed to handle RecipeDiaryCreatedDomainEvent: Failed to create DailyNutritionOverview for UserId={UserId}, Date={Date}. Error: {Error}",
                        notification.UserId,
                        notification.Date,
                        result.Error
                    );
                    throw new EventFailedException(
                        $"Failed to create DailyNutritionOverview for UserId {notification.UserId} and Date {notification.Date} when handling RecipeDiaryCreatedDomainEvent. Error: {result.Error}"
                    );
                }
                dailyNutritionOverview = result.Value;
            }

            dailyNutritionOverview.AddNutritionalValues(
                recipe.NutritionalContents,
                servingSize.NutritionMultiplier * notification.Quantity
            );

            await dailyNutritionOverviewRepository.AddAsync(
                dailyNutritionOverview,
                cancellationToken
            );
        }
        else
        {
            dailyNutritionOverview.AddNutritionalValues(
                recipe.NutritionalContents,
                servingSize.NutritionMultiplier * notification.Quantity
            );

            dailyNutritionOverviewRepository.Update(dailyNutritionOverview);
        }

        await nutritionUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
