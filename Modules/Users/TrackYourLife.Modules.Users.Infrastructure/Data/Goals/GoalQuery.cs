using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.Modules.Users.Infrastructure.Data.Goals.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Goals;

internal sealed class GoalQuery(UsersReadDbContext context) : IGoalQuery
{
    private readonly UsersReadDbContext _context = context;

    public async Task<GoalReadModel?> GetActiveGoalByTypeAsync(
        UserId userId,
        GoalType type,
        CancellationToken cancellationToken
    )
    {
        return await _context.UserGoals.FirstOrDefaultAsync(
            new ActiveUserGoalSpecification(userId, type).ToReadModelExpression(),
            cancellationToken
        );
    }
}
