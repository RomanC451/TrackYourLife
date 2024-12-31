using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Cms;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Infrastructure.Utils;

namespace TrackYourLife.Modules.Users.Infrastructure.Services;

public class AuthService(
    IJwtProvider jwtProvider,
    ITokenRepository userTokenRepository,
    IUsersUnitOfWork unitOfWork,
    IOptions<RefreshTokenCookieOptions> refreshTokenCookieOptions
) : IAuthService
{
    public async Task<Result<(string, Token)>> RefreshUserAuthTokensAsync(
        UserReadModel user,
        CancellationToken cancellationToken
    )
    {
        var jwtTokenString = jwtProvider.Generate(user);
        var refreshTokenString = TokenProvider.Generate();

        Token? refreshToken = await userTokenRepository.GetByUserIdAsync(
            user.Id,
            cancellationToken
        );

        var expiresAt = DateTime.UtcNow.AddDays(refreshTokenCookieOptions.Value.DaysToExpire);

        if (refreshToken is null)
        {
            var refreshTokenResult = Token.Create(
                TokenId.NewId(),
                refreshTokenString,
                user.Id,
                TokenType.RefreshToken,
                expiresAt
            );

            if (refreshTokenResult.IsFailure)
            {
                return Result.Failure<(string, Token)>(refreshTokenResult.Error);
            }

            refreshToken = refreshTokenResult.Value;

            await userTokenRepository.AddAsync(refreshToken, cancellationToken);
        }
        else
        {
            var updateResult = Result.FirstFailureOrSuccess(
                refreshToken.UpdateValue(refreshTokenString),
                refreshToken.UpdateExpiresAt(expiresAt)
            );

            if (updateResult.IsFailure)
            {
                return Result.Failure<(string, Token)>(updateResult.Error);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success((jwtTokenString, refreshToken));
    }

    public async Task<Result<string>> GenerateEmailVerificationLinkAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        var emailVerificationTokenResult = await GenerateEmailVerificationTokenAsync(
            userId,
            cancellationToken
        );

        if (emailVerificationTokenResult.IsFailure)
        {
            return Result.Failure<string>(emailVerificationTokenResult.Error);
        }

        var emailVerificationToken = emailVerificationTokenResult.Value;

        // TODO: fix it
#pragma warning disable S1075 // URIs should not be hardcoded
        var uriBuilder = new UriBuilder("http://192.168.1.8:5173/emailVerification");
#pragma warning restore S1075 // URIs should not be hardcoded
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["token"] = emailVerificationToken.Value;

        uriBuilder.Query = parameters.ToString();

        Uri verificationLink = uriBuilder.Uri;

        return Result.Success(verificationLink.ToString());
    }

    private async Task<Result<Token>> GenerateEmailVerificationTokenAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        Token? emailVerificationToken = await userTokenRepository.GetByUserIdAsync(
            userId,
            cancellationToken
        );

        var expiresAt = DateTime.UtcNow.AddMinutes(5);

        if (emailVerificationToken is null)
        {
            var emailVerificationTokenResult = Token.Create(
                TokenId.NewId(),
                TokenProvider.Generate(),
                userId,
                TokenType.EmailVerificationToken,
                expiresAt
            );

            if (emailVerificationTokenResult.IsFailure)
            {
                return Result.Failure<Token>(emailVerificationTokenResult.Error);
            }

            emailVerificationToken = emailVerificationTokenResult.Value;

            await userTokenRepository.AddAsync(emailVerificationToken, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        if (emailVerificationToken.ExpiresAt < DateTime.UtcNow)
        {
            var updateResult = emailVerificationToken.UpdateValue(TokenProvider.Generate());

            if (updateResult.IsFailure)
            {
                return Result.Failure<Token>(updateResult.Error);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(emailVerificationToken);
    }
}
