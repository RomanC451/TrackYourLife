using MassTransit;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Contracts.Integration.Busses;
using TrackYourLife.SharedLib.Contracts.Integration.Users;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;

public sealed class FoodDiaryCreatedDomainEventHandler(
    IFoodQuery foodQuery,
    IServingSizeQuery servingSizeQuery,
    IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
    INutritionUnitOfWork nutritionUnitOfWork,
    IUsersBus usersBus
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
            var client = usersBus.CreateRequestClient<GetNutritionGoalsByUserIdRequest>();

            var response = await client.GetResponse<
                GetNutritionGoalsByUserIdResponse,
                GetNutritionGoalsByUserIdErrorResponse
            >(
                new GetNutritionGoalsByUserIdRequest(notification.UserId, notification.Date),
                cancellationToken
            );

            if (response.Is(out Response<GetNutritionGoalsByUserIdErrorResponse>? errorResponse))
            {
                return;
            }
            else if (response.Is(out Response<GetNutritionGoalsByUserIdResponse>? goalsResponse))
            {
                var result = DailyNutritionOverview.Create(
                    DailyNutritionOverviewId.NewId(),
                    notification.UserId,
                    notification.Date,
                    goalsResponse.Message.CaloriesGoal,
                    goalsResponse.Message.CarbohydratesGoal,
                    goalsResponse.Message.FatGoal,
                    goalsResponse.Message.ProteinGoal
                );

                if (result.IsFailure)
                {
                    return;
                }
                dailyNutritionOverview = result.Value;
            }
            else
            {
                return;
            }

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
