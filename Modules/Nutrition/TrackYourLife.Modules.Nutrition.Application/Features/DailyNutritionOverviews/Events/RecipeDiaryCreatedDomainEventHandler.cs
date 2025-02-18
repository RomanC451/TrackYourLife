using MassTransit;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Contracts.Integration.Busses;
using TrackYourLife.SharedLib.Contracts.Integration.Users;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;

public class RecipeDiaryCreatedDomainEventHandler(
    IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
    IRecipeQuery recipeQuery,
    INutritionUnitOfWork nutritionUnitOfWork,
    IUsersBus usersBus
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
                recipe.NutritionalContents,
                notification.Quantity
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
                notification.Quantity
            );

            dailyNutritionOverviewRepository.Update(dailyNutritionOverview);
        }

        await nutritionUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
