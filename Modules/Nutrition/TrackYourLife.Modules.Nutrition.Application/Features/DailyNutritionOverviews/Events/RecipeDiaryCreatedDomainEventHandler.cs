using MassTransit;
using Serilog;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Contracts.Integration.Users;

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
            return;
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
            return;
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
                logger.Information(
                    "Failed to get nutrition goals. Errors: {Errors}",
                    response.Message.Errors
                );
                return;
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
                    return;
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
