using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Application.Abstractions.Authentication;

public interface IAuthService
{
    Task<(string, RefreshToken)> RefreshUserAuthTokens(
        User user,
        CancellationToken cancellationToken
    );

    Guid GetUserIdFromJwtToken(string jwtTokenValue);

    Result<bool> SetRefreshTokenCookie(RefreshToken refreshToken);

    Result<string> GetHttpContextJwtToken();
}
