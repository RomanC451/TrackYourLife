using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Domain.UserGoals;

public interface IUserGoalRepository
{
    Task<UserGoal?> GetByIdAsync(UserGoalId id, CancellationToken cancellationToken);

    Task AddAsync(UserGoal userGoal, CancellationToken cancellationToken);

    Task<UserGoal?> GetActiveGoalByTypeAsync(
        UserId userId,
        UserGoalType type,
        CancellationToken cancellationToken
    );
    Task<List<UserGoal>> GetOverlappingGoalsAsync(
        UserGoal goal,
        CancellationToken cancellationToken
    );
    void Remove(UserGoal userGoal);
    void Update(UserGoal userGoal);
}
