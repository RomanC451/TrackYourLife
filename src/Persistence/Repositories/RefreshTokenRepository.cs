using TrackYourLifeDotnet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.Entities;

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

    public async Task<UserToken?> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserTokens.FirstOrDefaultAsync(token => token.UserId == userId);
    }

    public void Add(UserToken token)
    {
        _context.UserTokens.Add(token);
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
