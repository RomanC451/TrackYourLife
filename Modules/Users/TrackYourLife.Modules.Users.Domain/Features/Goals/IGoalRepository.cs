using System.Linq.Expressions;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Features.Goals;

public interface IGoalRepository
{
    Task<Goal?> GetByIdAsync(GoalId id, CancellationToken cancellationToken);

    Task AddAsync(Goal userGoal, CancellationToken cancellationToken);

    Task<List<Goal>> GetOverlappingGoalsAsync(Goal goal, CancellationToken cancellationToken);
    void Remove(Goal userGoal);
    void Update(Goal userGoal);
    Task<Goal?> GetGoalByUserIdAndTypeAsync(
        UserId userId,
        GoalType type,
        CancellationToken cancellationToken
    );
}
