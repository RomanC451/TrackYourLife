using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetNutritionGoals;

internal sealed class GetNutritionGoalsQueryHandler(
    IGoalQuery userGoalQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetNutritionGoalsQuery, List<GoalReadModel>>
{
    public async Task<Result<List<GoalReadModel>>> Handle(
        GetNutritionGoalsQuery query,
        CancellationToken cancellationToken
    )
    {
        var caloriesGoal = await userGoalQuery.GetGoalByTypeAndDateAsync(
            userIdentifierProvider.UserId,
            GoalType.Calories,
            query.Date,
            cancellationToken
        );

        var proteinGoal = await userGoalQuery.GetGoalByTypeAndDateAsync(
            userIdentifierProvider.UserId,
            GoalType.Protein,
            query.Date,
            cancellationToken
        );

        var carbsGoal = await userGoalQuery.GetGoalByTypeAndDateAsync(
            userIdentifierProvider.UserId,
            GoalType.Carbohydrates,
            query.Date,
            cancellationToken
        );

        var fatsGoal = await userGoalQuery.GetGoalByTypeAndDateAsync(
            userIdentifierProvider.UserId,
            GoalType.Fats,
            query.Date,
            cancellationToken
        );

        if (caloriesGoal is null || proteinGoal is null || carbsGoal is null || fatsGoal is null)
        {
            return Result.Success<List<GoalReadModel>>([]);
        }

        return Result.Success<List<GoalReadModel>>(
            [caloriesGoal, proteinGoal, carbsGoal, fatsGoal]
        );
    }
}
