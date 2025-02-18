using System.Linq.Expressions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Goals;

public interface IGoalRepository
{
    Task<Goal?> GetByIdAsync(GoalId id, CancellationToken cancellationToken);

    Task AddAsync(Goal userGoal, CancellationToken cancellationToken);

    Task<Goal?> GetActiveGoalByTypeAsync(
        UserId userId,
        GoalType type,
        CancellationToken cancellationToken
    );
    Task<List<Goal>> GetOverlappingGoalsAsync(Goal goal, CancellationToken cancellationToken);
    void Remove(Goal userGoal);
    void Update(Goal userGoal);
    Task<Goal?> GetGoalByUserIdAndTypeAsync(
        UserId userId,
        GoalType type,
        CancellationToken cancellationToken
    );
}
