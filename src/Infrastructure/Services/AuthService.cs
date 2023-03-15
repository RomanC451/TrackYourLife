using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotNet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenProvider _refreshTokenProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HttpContext? _httpContext;

    private readonly CookieOptions _cookieOptions =
        new()
        {
            HttpOnly = true,
            Secure = false,
            Expires = null,
        };

    public AuthService(
        IRefreshTokenProvider refreshTokenProvider,
        IJwtProvider jwtProvider,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _refreshTokenProvider = refreshTokenProvider;
        _jwtProvider = jwtProvider;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<(string, RefreshToken)> RefreshUserAuthTokens(
        User user,
        CancellationToken cancellationToken
    )
    {
        var jwtTokenString = _jwtProvider.Generate(user);
        var refreshTokenString = _refreshTokenProvider.Generate();

        RefreshToken? refreshToken = await _refreshTokenRepository.GetByUserIdAsync(user.Id);

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

    public Result<bool> SetRefreshTokenCookie(RefreshToken refreshToken)
    {
        _cookieOptions.Expires = refreshToken.ExpiresAt;

        if (_httpContext is null)
        {
            return Result.Failure<bool>(InfrastructureErrors.HttpContext.NotExists);
        }

        _httpContext?.Response.Cookies.Append("refreshToken", refreshToken.Value, _cookieOptions);

        return Result.Success(true);
    }

    public Result<string> GetHttpContextJwtToken()
    {
        if (_httpContext?.Request?.Headers?.ContainsKey("Authorization") == false)
        {
            return Result.Failure<string>(InfrastructureErrors.HttpContext.NotExists);
        }

        string authorizationHeader = _httpContext!.Request.Headers["Authorization"].ToString();

        string[] headerParts = authorizationHeader.Split(' ');

        if (headerParts.Length < 2 || headerParts[0]?.ToLowerInvariant() != "bearer")
        {
            return Result.Failure<string>(DomainErrors.User.InvalidJwtToken);
        }

        string jwtToken = headerParts[1];

        return Result.Success(jwtToken);
    }
}
