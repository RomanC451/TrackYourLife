using MassTransit;
using Serilog;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.SharedLib.Contracts.Integration.Users.Events;
using TrackYourLife.SharedLib.Domain.OutboxMessages;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events
{
    public class NutritionGoalUpdatedIntegrationEventConsumer(
        IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
        INutritionUnitOfWork unitOfWork,
        ILogger logger
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
                var result = Result.FirstFailureOrSuccess(
                    overview.UpdateCaloriesGoal(nutritionGoalUpdatedIntegrationEvent.CaloriesGoal),
                    overview.UpdateCarbohydratesGoal(
                        nutritionGoalUpdatedIntegrationEvent.CarbohydratesGoal
                    ),
                    overview.UpdateFatGoal(nutritionGoalUpdatedIntegrationEvent.FatGoal),
                    overview.UpdateProteinGoal(nutritionGoalUpdatedIntegrationEvent.ProteinGoal)
                );

                if (result.IsFailure)
                {
                    logger.Error(
                        "Failed to update nutrition goal for user {UserId} and date {Date}. Error: {Error}",
                        nutritionGoalUpdatedIntegrationEvent.UserId,
                        nutritionGoalUpdatedIntegrationEvent.StartDate,
                        result.Error.ToString()
                    );
                    throw new MessageConsumerFailedException(result.Error.ToString());
                }

                dailyNutritionOverviewRepository.Update(overview);
            }

            await unitOfWork.SaveChangesAsync(context.CancellationToken);
        }
    }
}
