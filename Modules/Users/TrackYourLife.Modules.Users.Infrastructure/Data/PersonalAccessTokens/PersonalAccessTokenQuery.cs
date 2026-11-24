using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.PersonalAccessTokens;

internal sealed class PersonalAccessTokenQuery(UsersReadDbContext context)
    : GenericQuery<PersonalAccessTokenReadModel, PersonalAccessTokenId>(
        context.PersonalAccessTokens
    ),
        IPersonalAccessTokenQuery
{
    public async Task<IReadOnlyList<PersonalAccessTokenReadModel>> GetActiveByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        return await context
            .PersonalAccessTokens.Where(token =>
                token.UserId == userId && token.RevokedOnUtc == null
            )
            .OrderByDescending(token => token.CreatedOnUtc)
            .ToListAsync(cancellationToken);
    }
}
