using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;
using TrackYourLifeDotnet.Domain.Users;

namespace TrackYourLifeDotnet.Application.Abstractions.Services;

public interface IAuthService
{
    Task<(string, UserToken)> RefreshUserAuthTokensAsync(
        User user,
        CancellationToken cancellationToken
    );

    Result<Guid> GetUserIdFromJwtToken();

    Result SetRefreshTokenCookie(UserToken refreshToken);
    Result RemoveRefreshTokenCookie();

    Result<string> GetHttpContextJwtToken();

    Result<string> GetRefreshTokenFromCookie();

    Task<string> GenerateEmailVerificationLinkAsync(
        UserId userId,
        CancellationToken cancellationToken
    );
}
