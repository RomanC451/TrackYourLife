using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

public interface IAuthService
{
    Task<(string, UserToken)> RefreshUserAuthTokensAsync(
        User user,
        CancellationToken cancellationToken
    );

    Result<UserId> GetUserIdFromJwtToken();

    Result SetRefreshTokenCookie(UserToken refreshToken);
    Result RemoveRefreshTokenCookie();

    Result<string> GetHttpContextJwtToken();

    Result<string> GetRefreshTokenFromCookie();

    Task<string> GenerateEmailVerificationLinkAsync(
        UserId userId,
        CancellationToken cancellationToken
    );
}
