using System.IdentityModel.Tokens.Jwt;
using System.Web;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Infrastructure.Utils;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.Repositories;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HttpContext? _httpContext;

    private readonly CookieOptions _cookieOptions =
        new()
        {
            Expires = DateTime.UtcNow.AddDays(7),
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            //Domain = "192.168.1.6",
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

    public async Task<(string, UserToken)> RefreshUserAuthTokensAsync(
        User user,
        CancellationToken cancellationToken
    )
    {
        var jwtTokenString = _jwtProvider.Generate(user);
        var refreshTokenString = TokenProvider.Generate();

        UserToken? refreshToken = await _userTokenRepository.GetByUserIdAsync(
            user.Id,
            cancellationToken
        );

        if (refreshToken is null)
        {
            refreshToken = new(
                new UserTokenId(Guid.NewGuid()),
                refreshTokenString,
                user.Id,
                UserTokenTypes.RefreshToken
            );
            await _userTokenRepository.AddAsync(refreshToken, cancellationToken);
        }
        else
        {
            refreshToken.UpdateToken(refreshTokenString);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (jwtTokenString, refreshToken);
    }

    public Result<UserId> GetUserIdFromJwtToken()
    {
        string jwtTokenValue = GetHttpContextJwtToken().Value;

        if (string.IsNullOrEmpty(jwtTokenValue))
        {
            return Result.Failure<UserId>(DomainErrors.JwtToken.Invalid);
        }

        var jwtHandler = new JwtSecurityTokenHandler();

        if (!jwtHandler.CanReadToken(jwtTokenValue))
        {
            return Result.Failure<UserId>(DomainErrors.JwtToken.Invalid);
        }

        JwtSecurityToken jwtToken;
        Guid userId;

        try
        {
            jwtToken = jwtHandler.ReadJwtToken(jwtTokenValue);
            userId = Guid.Parse(jwtToken.Subject);
        }
        catch (ArgumentException)
        {
            return Result.Failure<UserId>(DomainErrors.JwtToken.Invalid);
        }

        return Result.Success(new UserId(userId));
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

    public Result RemoveRefreshTokenCookie()
    {
        if (_httpContext is null)
        {
            return Result.Failure(InfrastructureErrors.HttpContext.NotExists);
        }

        _cookieOptions.Expires = DateTime.UtcNow;

        _httpContext.Response.Cookies.Delete("refreshToken", _cookieOptions);

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
            return Result.Failure<string>(DomainErrors.RefreshToken.NotExisting);
        }
        return refreshTokenCookie;
    }

    public async Task<string> GenerateEmailVerificationLinkAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        UserToken? emailVerificationToken = await _userTokenRepository.GetByUserIdAsync(
            userId,
            cancellationToken
        );

        if (emailVerificationToken is null)
        {
            emailVerificationToken = new(
                new UserTokenId(Guid.NewGuid()),
                TokenProvider.Generate(),
                userId,
                UserTokenTypes.EmailVerificationToken
            );

            await _userTokenRepository.AddAsync(emailVerificationToken, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        if (emailVerificationToken.ExpiresAt < DateTime.UtcNow)
        {
            emailVerificationToken.UpdateToken(TokenProvider.Generate());
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var uriBuilder = new UriBuilder("http://192.168.1.8:5173/emailVerification");
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["token"] = emailVerificationToken.Value;

        uriBuilder.Query = parameters.ToString();

        Uri verificationLink = uriBuilder.Uri;

        return verificationLink.ToString();
    }
}
