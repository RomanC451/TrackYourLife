
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Tokens;

public interface ITokenQuery
{
    Task<TokenReadModel?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken);
    Task<TokenReadModel?> GetByValueAsync(string value, CancellationToken cancellationToken);
}
