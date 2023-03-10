using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Application.Users.Commands.RefreshJwtToken;

public class RefreshJwtTokenCommandHandler
    : ICommandHandler<RefreshJwtTokenCommand, RefreshJwtTokenResponse>
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenProvider _refreshTokenProvider;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshJwtTokenCommandHandler(
        IJwtProvider jwtProvider,
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IRefreshTokenProvider refreshTokenProvider,
        IUnitOfWork unitOfWork
    )
    {
        _jwtProvider = jwtProvider;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _refreshTokenProvider = refreshTokenProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RefreshJwtTokenResponse>> Handle(
        RefreshJwtTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        RefreshToken? refreshToken = await _refreshTokenRepository.GetByValueAsync(
            request.RefreshToken,
            cancellationToken
        );
        if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result.Failure<RefreshJwtTokenResponse>(
                refreshToken is null
                    ? DomainErrors.User.InvalidRefreshToken
                    : DomainErrors.User.ExpiredRefreshToken
            );
        }

        User? user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<RefreshJwtTokenResponse>(DomainErrors.User.InvalidRefreshToken);
        }

        var newJwtTokenString = _jwtProvider.Generate(user);

        var newRefreshTokenString = _refreshTokenProvider.Generate();

        refreshToken.UpdateToken(newRefreshTokenString);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshJwtTokenResponse(newJwtTokenString, refreshToken);
    }
}
