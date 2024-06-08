using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.UserGoals;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;
using TrackYourLife.Modules.Users.Infrastructure.Data.Specifications;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Queries;

internal sealed class UserGoalQuery(ApplicationReadDbContext context) : IUserGoalQuery
{
    private readonly ApplicationReadDbContext _context = context;

    public async Task<UserGoalReadModel?> GetActiveGoalByTypeAsync(
        UserId userId,
        UserGoalType type,
        CancellationToken cancellationToken
    )
    {
        return await _context.UserGoals
            .FirstOrDefaultAsync(new ActiveUserGoalSpecification(userId, type).ToReadModelExpression(), cancellationToken);
    }
}
