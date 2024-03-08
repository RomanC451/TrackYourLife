using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;
using TrackYourLifeDotnet.Domain.Users.Queries;

namespace TrackYourLifeDotnet.Persistence.Queries;

public class UserQuery : IUserQuery
{
    private readonly ApplicationDbContext _context;

    public UserQuery(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserExistsAsync(UserId userId, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(user => user.Id == userId, cancellationToken);
    }
}
