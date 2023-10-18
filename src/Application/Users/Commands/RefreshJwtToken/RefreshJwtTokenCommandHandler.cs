using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Application.Users.Commands.RefreshJwtToken;

public class RefreshJwtTokenCommandHandler
    : ICommandHandler<RefreshJwtTokenCommand, RefreshJwtTokenResponse>
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

    public async Task<Result<RefreshJwtTokenResponse>> Handle(
        RefreshJwtTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result.Failure<RefreshJwtTokenResponse>(DomainErrors.RefreshToken.Invalid);
        }

        UserToken? refreshToken = await _userTokenRepository.GetByValueAsync(
            request.RefreshToken,
            cancellationToken
        );
        if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result.Failure<RefreshJwtTokenResponse>(
                refreshToken is null
                    ? DomainErrors.RefreshToken.Invalid
                    : DomainErrors.RefreshToken.Expired
            );
        }

        User? user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<RefreshJwtTokenResponse>(DomainErrors.RefreshToken.Invalid);
        }

        (string jwtToken, UserToken newRefreshToken) = await _authService.RefreshUserAuthTokens(
            user,
            cancellationToken
        );

        _authService.SetRefreshTokenCookie(refreshToken);

        RefreshJwtTokenResponse response = new(jwtToken, newRefreshToken);

        return Result.Success(response);
    }
}
