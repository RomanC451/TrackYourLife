using MassTransit;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Consumers;

public class GetNutritionGoalsByUserIdConsumer(IGoalQuery goalQuery)
    : IConsumer<GetNutritionGoalsByUserIdRequest>
{
    public async Task Consume(ConsumeContext<GetNutritionGoalsByUserIdRequest> context)
    {
        var caloriesGoal = await goalQuery.GetGoalByTypeAndDateAsync(
            context.Message.UserId,
            GoalType.Calories,
            context.Message.Date,
            context.CancellationToken
        );

        var carbohydratesGoal = await goalQuery.GetGoalByTypeAndDateAsync(
            context.Message.UserId,
            GoalType.Carbohydrates,
            context.Message.Date,
            context.CancellationToken
        );

        var fatGoal = await goalQuery.GetGoalByTypeAndDateAsync(
            context.Message.UserId,
            GoalType.Fats,
            context.Message.Date,
            context.CancellationToken
        );

        var proteinGoal = await goalQuery.GetGoalByTypeAndDateAsync(
            context.Message.UserId,
            GoalType.Protein,
            context.Message.Date,
            context.CancellationToken
        );

        List<Error> errors = [];

        if (caloriesGoal is null)
        {
            errors.Add(GoalErrors.NotExisting(GoalType.Calories));
        }

        if (carbohydratesGoal is null)
        {
            errors.Add(GoalErrors.NotExisting(GoalType.Carbohydrates));
        }

        if (fatGoal is null)
        {
            errors.Add(GoalErrors.NotExisting(GoalType.Fats));
        }

        if (proteinGoal is null)
        {
            errors.Add(GoalErrors.NotExisting(GoalType.Protein));
        }

        if (
            caloriesGoal is null
            || carbohydratesGoal is null
            || fatGoal is null
            || proteinGoal is null
        )
        {
            await context.RespondAsync(new GetNutritionGoalsByUserIdErrorResponse(errors));
            return;
        }

        var response = new GetNutritionGoalsByUserIdResponse(
            context.Message.UserId,
            context.Message.Date,
            caloriesGoal.Value,
            carbohydratesGoal.Value,
            fatGoal.Value,
            proteinGoal.Value
        );

        await context.RespondAsync(response);
    }
}
