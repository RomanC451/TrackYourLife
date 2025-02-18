using Microsoft.AspNetCore.Http.Features;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetNutritionActiveGoals;

public sealed class GetActiveNutritionGoalsQueryHandler(
    IGoalQuery userGoalQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetActiveNutritionGoalsQuery, List<GoalReadModel>>
{
    public async Task<Result<List<GoalReadModel>>> Handle(
        GetActiveNutritionGoalsQuery query,
        CancellationToken cancellationToken
    )
    {
        var caloriesGoal = await userGoalQuery.GetActiveGoalByTypeAsync(
            userIdentifierProvider.UserId,
            GoalType.Calories,
            cancellationToken
        );

        var proteinGoal = await userGoalQuery.GetActiveGoalByTypeAsync(
            userIdentifierProvider.UserId,
            GoalType.Protein,
            cancellationToken
        );

        var carbsGoal = await userGoalQuery.GetActiveGoalByTypeAsync(
            userIdentifierProvider.UserId,
            GoalType.Carbohydrates,
            cancellationToken
        );

        var fatsGoal = await userGoalQuery.GetActiveGoalByTypeAsync(
            userIdentifierProvider.UserId,
            GoalType.Fats,
            cancellationToken
        );

        if (caloriesGoal is null)
        {
            return Result.Failure<List<GoalReadModel>>(GoalErrors.NotExisting(GoalType.Calories));
        }
        if (proteinGoal is null)
        {
            return Result.Failure<List<GoalReadModel>>(GoalErrors.NotExisting(GoalType.Protein));
        }
        if (carbsGoal is null)
        {
            return Result.Failure<List<GoalReadModel>>(
                GoalErrors.NotExisting(GoalType.Carbohydrates)
            );
        }
        if (fatsGoal is null)
        {
            return Result.Failure<List<GoalReadModel>>(GoalErrors.NotExisting(GoalType.Fats));
        }

        return Result.Success<List<GoalReadModel>>(
            [caloriesGoal, proteinGoal, carbsGoal, fatsGoal]
        );
    }
}
