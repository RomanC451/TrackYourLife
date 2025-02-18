using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Goals;

public interface IGoalQuery
{
    Task<GoalReadModel?> GetActiveGoalByTypeAsync(
        UserId userId,
        GoalType type,
        CancellationToken cancellationToken
    );

    Task<GoalReadModel?> GetGoalByTypeAndDateAsync(
        UserId userId,
        GoalType type,
        DateOnly date,
        CancellationToken cancellationToken
    );
}
