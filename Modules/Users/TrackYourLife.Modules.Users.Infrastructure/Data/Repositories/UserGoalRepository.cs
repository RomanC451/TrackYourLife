
using TrackYourLife.Common.Persistence.Repositories;
using TrackYourLife.Modules.Users.Domain.UserGoals;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;
using TrackYourLife.Modules.Users.Infrastructure.Data.Specifications;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Repositories;

internal sealed class UserGoalRepository(ApplicationWriteDbContext context)
        : GenericRepository<UserGoal, UserGoalId>(context.UserGoals),
        IUserGoalRepository
{
    public async Task<UserGoal?> GetActiveGoalByTypeAsync(
        UserId userId,
        UserGoalType type,
        CancellationToken cancellationToken
    )
    {
        return await FirstOrDefaultAsync(
            new ActiveUserGoalSpecification(userId, type),
            cancellationToken
        );
    }

    public async Task<List<UserGoal>> GetOverlappingGoalsAsync(
        UserGoal goal,
        CancellationToken cancellationToken
    )
    {
        return (
            await WhereAsync(new UserGoalOverlappingOtherGoalSpecification(goal), cancellationToken)
        )
            .OrderBy(g => g.StartDate)
            .ToList();
    }
}
