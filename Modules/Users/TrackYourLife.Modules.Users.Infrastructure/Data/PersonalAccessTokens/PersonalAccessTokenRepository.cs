using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.PersonalAccessTokens;

internal sealed class PersonalAccessTokenRepository(UsersWriteDbContext context)
    : GenericRepository<PersonalAccessToken, PersonalAccessTokenId>(context.PersonalAccessTokens),
        IPersonalAccessTokenRepository
{
    public async Task<PersonalAccessToken?> GetActiveByTokenHashAsync(
        string tokenHash,
        DateTime utcNow,
        CancellationToken cancellationToken
    )
    {
        return await context.PersonalAccessTokens.FirstOrDefaultAsync(
            token =>
                token.TokenHash == tokenHash
                && token.RevokedOnUtc == null
                && token.ExpiresAt > utcNow,
            cancellationToken
        );
    }
}
