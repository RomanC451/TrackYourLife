using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Features.Tokens;

public interface ITokenQuery
{
    Task<IEnumerable<TokenReadModel>> GetByUserIdAndTypeAsync(
        UserId userId,
        TokenType tokenType,
        CancellationToken cancellationToken
    );
    Task<TokenReadModel?> GetByValueAsync(string value, CancellationToken cancellationToken);
}
