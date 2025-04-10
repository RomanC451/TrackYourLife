using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateNutritionGoals;

internal sealed class UpdateNutritionGoalsCommandHandler(
    IGoalRepository goalRepository,
    IGoalsManagerService goalsManagerService,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateNutritionGoalsCommand>
{
    public async Task<Result> Handle(
        UpdateNutritionGoalsCommand command,
        CancellationToken cancellationToken
    )
    {
        var goalResults = UpdateNutritionGoals(command, userIdentifierProvider.UserId);

        foreach (var goalResult in goalResults)
        {
            if (goalResult.IsFailure)
                return Result.Failure(goalResult.Error);

            var goal = goalResult.Value;

            var result = await goalsManagerService.HandleOverlappingGoalsAsync(
                goal,
                command.Force,
                cancellationToken
            );
            if (result.IsFailure)
                return result;

            var skip = result.Value;
            if (skip)
                continue;

            await goalRepository.AddAsync(goal, cancellationToken);
        }
        return Result.Success();
    }

    private static List<Result<Goal>> UpdateNutritionGoals(
        UpdateNutritionGoalsCommand command,
        UserId userId
    )
    {
        var calorieGoalResult = Goal.Create(
            GoalId.NewId(),
            userId,
            GoalType.Calories,
            command.Calories,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var proteinGoalResult = Goal.Create(
            GoalId.NewId(),
            userId,
            GoalType.Protein,
            command.Protein,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var carbsGoalResult = Goal.Create(
            GoalId.NewId(),
            userId,
            GoalType.Carbohydrates,
            command.Carbohydrates,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var fatGoalResult = Goal.Create(
            GoalId.NewId(),
            userId,
            GoalType.Fats,
            command.Fats,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        return [calorieGoalResult, proteinGoalResult, carbsGoalResult, fatGoalResult];
    }
}
