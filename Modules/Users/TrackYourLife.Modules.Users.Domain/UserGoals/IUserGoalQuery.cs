using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Domain.UserGoals;

public interface IUserGoalQuery
{
    Task<UserGoalReadModel?> GetActiveGoalByTypeAsync(
        UserId userId,
        UserGoalType type,
        CancellationToken cancellationToken
    );
}
