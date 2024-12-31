using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.Modules.Users.Infrastructure.Data.Tokens.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Tokens;

internal sealed class TokenQuery(UsersReadDbContext context) : GenericQuery<TokenReadModel
    , TokenId>(context.Tokens), ITokenQuery
{


    public async Task<TokenReadModel?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken)
    {
        return await context.Tokens.FirstOrDefaultAsync(new TokenReadModelWithUserIdSpecification(userId), cancellationToken);
    }

    public async Task<TokenReadModel?> GetByValueAsync(string value, CancellationToken cancellationToken)
    {
        return await context.Tokens.FirstOrDefaultAsync(new TokenReadModelWithValueSpecification(value), cancellationToken);
    }
}
