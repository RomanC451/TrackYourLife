using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.Repositories;
using TrackYourLifeDotnet.Domain.Users;

namespace TrackYourLifeDotnet.Application.Users.Commands.RefreshJwtToken;

public sealed class RefreshJwtTokenCommandHandler
    : ICommandHandler<RefreshJwtTokenCommand, RefreshJwtTokenResult>
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

    public async Task<Result<RefreshJwtTokenResult>> Handle(
        RefreshJwtTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result.Failure<RefreshJwtTokenResult>(DomainErrors.RefreshToken.Invalid);
        }

        UserToken? refreshToken = await _userTokenRepository.GetByValueAsync(
            command.RefreshToken,
            cancellationToken
        );
        if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result.Failure<RefreshJwtTokenResult>(
                refreshToken is null
                    ? DomainErrors.RefreshToken.NotExisting
                    : DomainErrors.RefreshToken.Expired
            );
        }

        User? user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<RefreshJwtTokenResult>(DomainErrors.RefreshToken.Invalid);
        }

        (string jwtToken, UserToken newRefreshToken) =
            await _authService.RefreshUserAuthTokensAsync(user, cancellationToken);

        _authService.SetRefreshTokenCookie(newRefreshToken);

        RefreshJwtTokenResult response = new(jwtToken);

        return Result.Success(response);
    }
}
