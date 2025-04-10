using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Infrastructure.Data.Goals.Specifications;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Goals;

internal sealed class GoalQuery(UsersReadDbContext context)
    : GenericQuery<GoalReadModel, GoalId>(context.Goals),
        IGoalQuery
{
    private readonly UsersReadDbContext _context = context;

    public async Task<GoalReadModel?> GetGoalByTypeAndDateAsync(
        UserId userId,
        GoalType type,
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        return await _context.Goals.FirstOrDefaultAsync(
            new UserGoalWithTypeAndDateSpecification(userId, type, date).ToReadModelExpression(),
            cancellationToken
        );
    }
}
