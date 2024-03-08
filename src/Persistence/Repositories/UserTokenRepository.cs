using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.Users;
using TrackYourLifeDotnet.Domain.Users.Repositories;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Persistence.Repositories;

public sealed class UserTokenRepository : IUserTokenRepository
{
    private readonly ApplicationDbContext _context;

    public UserTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserToken?> GetByValueAsync(string value, CancellationToken cancellationToken)
    {
        return await _context.UserTokens.FirstOrDefaultAsync(
            token => token.Value == value,
            cancellationToken
        );
    }

    public async Task<UserToken?> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        return await _context.UserTokens.FirstOrDefaultAsync(
            token => token.UserId == userId,
            cancellationToken
        );
    }

    public async Task AddAsync(UserToken token, CancellationToken cancellationToken)
    {
        await _context.UserTokens.AddAsync(token, cancellationToken);
    }

    public void Remove(UserToken token)
    {
        _context.UserTokens.Remove(token);
    }

    public void Update(UserToken token)
    {
        _context.UserTokens.Update(token);
    }
}
