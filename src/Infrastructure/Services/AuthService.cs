using System.IdentityModel.Tokens.Jwt;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Repositories;

namespace TrackYourLifeDotnet.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenProvider _refreshTokenProvider;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IRefreshTokenProvider refreshTokenProvider,
        IJwtProvider jwtProvider,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork
    )
    {
        _refreshTokenProvider = refreshTokenProvider;
        _jwtProvider = jwtProvider;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<(string, RefreshToken)> RefreshUserAuthTokens(
        User user,
        CancellationToken cancellationToken
    )
    {
        var jwtTokenString = _jwtProvider.Generate(user);
        var refreshTokenString = _refreshTokenProvider.Generate();

        RefreshToken? refreshToken = await _refreshTokenRepository.GetByUserId(user.Id);

        if (refreshToken is null)
        {
            refreshToken = new(Guid.NewGuid(), refreshTokenString, user.Id);
            _refreshTokenRepository.Add(refreshToken);
        }
        else
        {
            refreshToken.UpdateToken(refreshTokenString);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (jwtTokenString, refreshToken);
    }

    public Guid GetUserIdFromJwtToken(string jwtTokenValue)
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtTokenValue);

        return Guid.Parse(jwtToken.Subject);
    }
}
