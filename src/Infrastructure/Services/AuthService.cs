using System.IdentityModel.Tokens.Jwt;
using System.Web;
using Microsoft.AspNetCore.Http;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Infrastructure.utils;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Enums;

namespace TrackYourLifeDotnet.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IJwtProvider _jwtProvider;
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
        IJwtProvider jwtProvider,
        IUserTokenRepository userTokenRepository,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _jwtProvider = jwtProvider;
        _userTokenRepository = userTokenRepository;
        _unitOfWork = unitOfWork;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<(string, UserToken)> RefreshUserAuthTokens(
        User user,
        CancellationToken cancellationToken
    )
    {
        var jwtTokenString = _jwtProvider.Generate(user);
        var refreshTokenString = TokenProvider.Generate();

        UserToken? refreshToken = await _userTokenRepository.GetByUserIdAsync(user.Id);

        if (refreshToken is null)
        {
            refreshToken = new(
                Guid.NewGuid(),
                refreshTokenString,
                user.Id,
                UserTokenTypes.RefreshToken
            );
            _userTokenRepository.Add(refreshToken);
        }
        else
        {
            refreshToken.UpdateToken(refreshTokenString);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (jwtTokenString, refreshToken);
    }

    public Result<Guid> GetUserIdFromJwtToken(string jwtTokenValue)
    {
        if (string.IsNullOrEmpty(jwtTokenValue))
        {
            return Result.Failure<Guid>(DomainErrors.JwtToken.Invalid);
        }

        var jwtHandler = new JwtSecurityTokenHandler();

        if (!jwtHandler.CanReadToken(jwtTokenValue))
        {
            return Result.Failure<Guid>(DomainErrors.JwtToken.Invalid);
        }

        JwtSecurityToken jwtToken;

        try
        {
            jwtToken = jwtHandler.ReadJwtToken(jwtTokenValue);
        }
        catch (ArgumentException)
        {
            return Result.Failure<Guid>(DomainErrors.JwtToken.Invalid);
        }

        return Result.Success(Guid.Parse(jwtToken.Subject));
    }

    public Result SetRefreshTokenCookie(UserToken refreshToken)
    {
        _cookieOptions.Expires = refreshToken.ExpiresAt;

        if (string.IsNullOrEmpty(refreshToken.Value))
        {
            return Result.Failure(DomainErrors.RefreshToken.Invalid);
        }

        if (_httpContext is null)
        {
            return Result.Failure(InfrastructureErrors.HttpContext.NotExists);
        }

        _httpContext.Response.Cookies.Append("refreshToken", refreshToken.Value, _cookieOptions);

        return Result.Success();
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
            return Result.Failure<string>(DomainErrors.JwtToken.Invalid);
        }

        string jwtToken = headerParts[1];

        return Result.Success(jwtToken);
    }

    public Result<string> GetRefreshTokenFromCookie()
    {
        if (_httpContext is null)
        {
            return Result.Failure<string>(InfrastructureErrors.HttpContext.NotExists);
        }

        var refreshTokenCookie = _httpContext.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshTokenCookie))
        {
            return Result.Failure<string>(DomainErrors.RefreshToken.Invalid);
        }
        return refreshTokenCookie;
    }

    public async Task<string> GenerateEmailVerificationLink(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        UserToken emailVerificationToken =
            new(
                Guid.NewGuid(),
                TokenProvider.Generate(),
                userId,
                UserTokenTypes.EmailVerificationToken
            );

        _userTokenRepository.Add(emailVerificationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var uriBuilder = new UriBuilder("https://192.168.1.8:44497/email-verification");
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["token"] = emailVerificationToken.Value;

        uriBuilder.Query = parameters.ToString();

        Uri verificationLink = uriBuilder.Uri;

        return verificationLink.ToString();
    }
}
