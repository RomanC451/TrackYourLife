using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Infrastructure.Data.Goals.Specifications;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Goals;

internal sealed class GoalRepository(UsersWriteDbContext context)
    : GenericRepository<Goal, GoalId>(context.Goals),
        IGoalRepository
{
    public async Task<List<Goal>> GetOverlappingGoalsAsync(
        Goal goal,
        CancellationToken cancellationToken
    )
    {
        return (
            await WhereAsync(new GoalOverlappingOtherGoalSpecification(goal), cancellationToken)
        )
            .OrderBy(g => g.StartDate)
            .ToList();
    }

    public async Task<Goal?> GetGoalByUserIdAndTypeAsync(
        UserId userId,
        GoalType type,
        CancellationToken cancellationToken
    )
    {
        return await FirstOrDefaultAsync(
            new GoalWithTypeSpecification(userId, type),
            cancellationToken
        );
    }
}
