using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Application.Abstractions.Authentication;

public interface IAuthService
{
    Task<(string, RefreshToken)> RefreshUserAuthTokens(
        User user,
        CancellationToken cancellationToken
    );

    Guid GetUserIdFromJwtToken(string jwtTokenValue);
}
