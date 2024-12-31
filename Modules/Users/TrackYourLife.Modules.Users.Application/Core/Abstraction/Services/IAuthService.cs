using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

public interface IAuthService
{
    Task<Result<(string, Token)>> RefreshUserAuthTokensAsync(
        UserReadModel user,
        CancellationToken cancellationToken
    );

    Task<Result<string>> GenerateEmailVerificationLinkAsync(
        UserId userId,
        CancellationToken cancellationToken
    );
}
