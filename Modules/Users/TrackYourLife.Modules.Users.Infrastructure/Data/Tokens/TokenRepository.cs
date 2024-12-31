using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.Modules.Users.Infrastructure.Data.Tokens.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Tokens;

internal sealed class TokenRepository(UsersWriteDbContext context)
    : GenericRepository<Token, TokenId>(context.Tokens),
        ITokenRepository
{
    public async Task<Token?> GetByValueAsync(string value, CancellationToken cancellationToken) =>
        await FirstOrDefaultAsync(new TokenWithValueSpecification(value), cancellationToken);

    public async Task<Token?> GetByUserIdAsync(UserId id, CancellationToken cancellationToken)
    {
        return await FirstOrDefaultAsync(
            new TokenWithUserIdSpecification(id),
            cancellationToken
        );
    }
}
