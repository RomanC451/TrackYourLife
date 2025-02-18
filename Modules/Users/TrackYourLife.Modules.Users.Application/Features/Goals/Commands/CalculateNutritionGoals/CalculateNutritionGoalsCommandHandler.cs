using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;

public sealed class CalculateNutritionGoalsCommandHandler(
    IGoalRepository goalRepository,
    IGoalsManagerService goalsManagerService,
    INutritionCalculator nutritionCalculator,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<CalculateNutritionGoalsCommand>
{
    public async Task<Result> Handle(
        CalculateNutritionGoalsCommand command,
        CancellationToken cancellationToken
    )
    {
        var goalResults = CalculateNutritionGoals(command, userIdentifierProvider.UserId);

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

            await goalRepository.AddAsync(goal, cancellationToken);
        }

        return Result.Success();
    }

    private List<Result<Goal>> CalculateNutritionGoals(
        CalculateNutritionGoalsCommand command,
        UserId userId
    )
    {
        var calories = nutritionCalculator.CalculateCalories(
            command.Age,
            command.Weight,
            command.Height,
            command.Gender,
            command.ActivityLevel,
            command.FitnessGoal
        );

        var calorieGoalResult = Goal.Create(
            GoalId.NewId(),
            userId,
            GoalType.Calories,
            calories,
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var proteinGoalResult = Goal.Create(
            GoalId.NewId(),
            userId,
            GoalType.Protein,
            nutritionCalculator.CalculateProtein(command.Weight),
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var carbsGoalResult = Goal.Create(
            GoalId.NewId(),
            userId,
            GoalType.Carbohydrates,
            nutritionCalculator.CalculateCarbs(calories),
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var fatGoalResult = Goal.Create(
            GoalId.NewId(),
            userId,
            GoalType.Fats,
            nutritionCalculator.CalculateFat(calories),
            GoalPeriod.Day,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        return [calorieGoalResult, proteinGoalResult, carbsGoalResult, fatGoalResult];
    }
}
