using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Application.Abstractions.Authentication;

public interface IAuthService
{
    Task<(string, RefreshToken)> RefreshUserAuthTokens(
        User user,
        CancellationToken cancellationToken
    );

    Result<Guid> GetUserIdFromJwtToken(string jwtTokenValue);

    Result SetRefreshTokenCookie(RefreshToken refreshToken);

    Result<string> GetHttpContextJwtToken();

    Result<string> GetRefreshTokenFromCookie();
}
