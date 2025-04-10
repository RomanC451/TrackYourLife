using MassTransit;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.SharedLib.Contracts.Integration.Users.Events;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events
{
    public class NutritionGoalUpdatedIntegrationEventConsummer(
        IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
        INutritionUnitOfWork unitOfWork
    ) : IConsumer<NutritionGoalUpdatedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<NutritionGoalUpdatedIntegrationEvent> context)
        {
            var nutritionGoalUpdatedIntegrationEvent = context.Message;

            var dailyNutritionOverviews =
                await dailyNutritionOverviewRepository.GetByUserIdAndDateRangeAsync(
                    nutritionGoalUpdatedIntegrationEvent.UserId,
                    nutritionGoalUpdatedIntegrationEvent.StartDate,
                    nutritionGoalUpdatedIntegrationEvent.EndDate,
                    context.CancellationToken
                );

            if (!dailyNutritionOverviews.Any())
            {
                return;
            }

            foreach (var overview in dailyNutritionOverviews)
            {
                var result = nutritionGoalUpdatedIntegrationEvent.Type switch
                {
                    GoalType.Calories => overview.UpdateCaloriesGoal(
                        nutritionGoalUpdatedIntegrationEvent.Value
                    ),
                    GoalType.Carbohydrates => overview.UpdateCarbohydratesGoal(
                        nutritionGoalUpdatedIntegrationEvent.Value
                    ),
                    GoalType.Fats => overview.UpdateFatGoal(
                        nutritionGoalUpdatedIntegrationEvent.Value
                    ),
                    GoalType.Protein => overview.UpdateProteinGoal(
                        nutritionGoalUpdatedIntegrationEvent.Value
                    ),
                    _ => Result.Failure(
                        DomainErrors.ArgumentError.Invalid(
                            nameof(nutritionGoalUpdatedIntegrationEvent),
                            nameof(nutritionGoalUpdatedIntegrationEvent.Type)
                        )
                    ),
                };

                if (result.IsFailure)
                {
                    return;
                }

                dailyNutritionOverviewRepository.Update(overview);
            }

            await unitOfWork.SaveChangesAsync(context.CancellationToken);
        }
    }
}
