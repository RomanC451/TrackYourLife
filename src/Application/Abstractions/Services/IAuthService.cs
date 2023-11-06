using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Application.Abstractions.Services;

public interface IAuthService
{
    Task<(string, UserToken)> RefreshUserAuthTokens(User user, CancellationToken cancellationToken);

    Result<Guid> GetUserIdFromJwtToken();

    Result SetRefreshTokenCookie(UserToken refreshToken);

    Result<string> GetHttpContextJwtToken();

    Result<string> GetRefreshTokenFromCookie();

    Task<string> GenerateEmailVerificationLink(Guid userId, CancellationToken cancellationToken);
}
