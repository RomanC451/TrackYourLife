using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Application.Core.Abstractions.Services;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users;
using TrackYourLife.Common.Domain.Users.Repositories;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Contracts.Users;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.RefreshJwtToken;

public sealed class RefreshJwtTokenCommandHandler
    : ICommandHandler<RefreshJwtTokenCommand, TokenResponse>
{
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public RefreshJwtTokenCommandHandler(
        IUserTokenRepository userTokenRepository,
        IUserRepository userRepository,
        IAuthService authService
    )
    {
        _userTokenRepository = userTokenRepository;
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<Result<TokenResponse>> Handle(
        RefreshJwtTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var refreshTokenResult = _authService.GetRefreshTokenFromCookie();

        if (refreshTokenResult.IsFailure)
        {
            return Result.Failure<TokenResponse>(refreshTokenResult.Error);
        }

        UserToken? refreshToken = await _userTokenRepository.GetByValueAsync(
            refreshTokenResult.Value,
            cancellationToken
        );
        if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result.Failure<TokenResponse>(
                refreshToken is null
                    ? DomainErrors.RefreshToken.NotExisting
                    : DomainErrors.RefreshToken.Expired
            );
        }

        User? user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<TokenResponse>(DomainErrors.RefreshToken.Invalid);
        }

        (string jwtToken, UserToken newRefreshToken) =
            await _authService.RefreshUserAuthTokensAsync(user, cancellationToken);

        _authService.SetRefreshTokenCookie(newRefreshToken);

        return Result.Success(new TokenResponse(jwtToken));
    }
}
