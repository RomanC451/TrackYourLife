using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.Users.Queries;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;
using TrackYourLife.Modules.Users.Infrastructure;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Queries;

internal class UserQuery(ApplicationReadDbContext context) : IUserQuery
{
    private readonly ApplicationReadDbContext _context = context;

    public async Task<bool> UserExistsAsync(UserId userId, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(user => user.Id == userId.Value, cancellationToken);
    }
}
