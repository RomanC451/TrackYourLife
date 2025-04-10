using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Features.Goals;

public interface IGoalQuery
{
    Task<GoalReadModel?> GetByIdAsync(GoalId goalId, CancellationToken cancellationToken);

    Task<GoalReadModel?> GetGoalByTypeAndDateAsync(
        UserId userId,
        GoalType type,
        DateOnly date,
        CancellationToken cancellationToken
    );
}
