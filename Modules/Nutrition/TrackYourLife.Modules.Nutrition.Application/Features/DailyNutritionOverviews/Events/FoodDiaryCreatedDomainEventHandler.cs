using MassTransit;
using Serilog;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Contracts.Integration.Users;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;

internal sealed class FoodDiaryCreatedDomainEventHandler(
    IFoodQuery foodQuery,
    IServingSizeQuery servingSizeQuery,
    IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
    INutritionUnitOfWork nutritionUnitOfWork,
    IBus bus,
    ILogger logger
) : IDomainEventHandler<FoodDiaryCreatedDomainEvent>
{
    public async Task Handle(
        FoodDiaryCreatedDomainEvent notification,
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
            var client = bus.CreateRequestClient<GetNutritionGoalsByUserIdRequest>();

            var response = await client.GetResponse<GetNutritionGoalsByUserIdResponse>(
                new GetNutritionGoalsByUserIdRequest(notification.UserId, notification.Date),
                cancellationToken
            );
            Result<DailyNutritionOverview> result;

            if (response.Message.Data is null)
            {
                logger.Information(
                    "Failed to get nutrition goals. Errors: {Errors}",
                    response.Message.Errors
                );

                result = DailyNutritionOverview.Create(
                    DailyNutritionOverviewId.NewId(),
                    notification.UserId,
                    notification.Date,
                    0,
                    0,
                    0,
                    0
                );
            }
            else
            {
                result = DailyNutritionOverview.Create(
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
            }

            if (result.IsFailure)
            {
                return;
            }

            dailyNutritionOverview = result.Value;

            dailyNutritionOverview.AddNutritionalValues(
                food.NutritionalContents,
                notification.Quantity * servingSize.NutritionMultiplier
            );

            await dailyNutritionOverviewRepository.AddAsync(
                dailyNutritionOverview,
                cancellationToken
            );
        }
        else
        {
            dailyNutritionOverview.AddNutritionalValues(
                food.NutritionalContents,
                notification.Quantity * servingSize.NutritionMultiplier
            );

            dailyNutritionOverviewRepository.Update(dailyNutritionOverview);
        }

        await nutritionUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
