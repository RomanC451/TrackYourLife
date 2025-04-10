using TrackYourLife.Modules.Users.Domain.Features.Tokens;
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

    public async Task<List<Token>> GetByUserIdAndTypeAsync(
        UserId id,
        TokenType tokenType,
        CancellationToken cancellationToken
    )
    {
        return await WhereAsync(
            new TokenWithUserIdAndTypeSpecification(id, tokenType),
            cancellationToken
        );
    }
}
