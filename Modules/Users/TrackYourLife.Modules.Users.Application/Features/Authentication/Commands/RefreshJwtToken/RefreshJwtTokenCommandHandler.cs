using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RefreshJwtToken;

internal sealed class RefreshJwtTokenCommandHandler(
    ITokenQuery tokenQuery,
    IUserQuery userQuery,
    IAuthService authService
) : ICommandHandler<RefreshJwtTokenCommand, (TokenResponse, Token)>
{
    public async Task<Result<(TokenResponse, Token)>> Handle(
        RefreshJwtTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var refreshToken = await tokenQuery.GetByValueAsync(
            command.RefreshTokenValue,
            cancellationToken
        );

        if (refreshToken is null)
        {
            return Result.Failure<(TokenResponse, Token)>(TokenErrors.RefreshToken.Invalid);
        }

        if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result.Failure<(TokenResponse, Token)>(TokenErrors.RefreshToken.Expired);
        }

        var user = await userQuery.GetByIdAsync(refreshToken.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<(TokenResponse, Token)>(TokenErrors.RefreshToken.Invalid);
        }

        var refreshTokensResult = await authService.RefreshUserAuthTokensAsync(
            user,
            command.DeviceId,
            cancellationToken
        );

        if (refreshTokensResult.IsFailure)
        {
            return Result.Failure<(TokenResponse, Token)>(refreshTokensResult.Error);
        }

        (string jwtToken, Token newRefreshToken) = refreshTokensResult.Value;

        return Result.Success((new TokenResponse(jwtToken), newRefreshToken));
    }
}
