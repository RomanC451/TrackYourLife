using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Errors;

namespace TrackYourLifeDotnet.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByValueAsync(
        string value,
        CancellationToken cancellationToken
    )
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(
            token => token.Value == value,
            cancellationToken
        );
    }

    public async Task<RefreshToken?> GetByUserIdAsync(Guid id)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(token => token.UserId == id);
    }

    public void Add(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
    }

    public void Remove(RefreshToken token)
    {
        _context.RefreshTokens.Remove(token);
    }

    public void Update(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
    }
}
