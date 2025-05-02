using System.Web;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Infrastructure.Utils;

namespace TrackYourLife.Modules.Users.Infrastructure.Services;

internal sealed class AuthService(
    IJwtProvider jwtProvider,
    ITokenRepository tokenRepository,
    IUsersUnitOfWork unitOfWork,
    IOptions<RefreshTokenCookieOptions> refreshTokenCookieOptions,
    IOptions<ClientAppOptions> clientAppOptions
) : IAuthService
{
    public async Task<Result<(string, Token)>> RefreshUserAuthTokensAsync(
        UserReadModel user,
        DeviceId deviceId,
        CancellationToken cancellationToken
    )
    {
        var jwtTokenString = jwtProvider.Generate(user);
        var refreshTokenString = TokenProvider.Generate();

        var refreshToken = (
            await tokenRepository.GetByUserIdAndTypeAsync(
                user.Id,
                TokenType.RefreshToken,
                cancellationToken
            )
        ).Find(t => t.DeviceId == deviceId);

        var expiresAt = DateTime.UtcNow.AddDays(refreshTokenCookieOptions.Value.DaysToExpire);

        if (refreshToken is null)
        {
            var refreshTokenResult = Token.Create(
                TokenId.NewId(),
                refreshTokenString,
                user.Id,
                TokenType.RefreshToken,
                expiresAt,
                deviceId
            );

            if (refreshTokenResult.IsFailure)
            {
                return Result.Failure<(string, Token)>(refreshTokenResult.Error);
            }

            refreshToken = refreshTokenResult.Value;

            await tokenRepository.AddAsync(refreshToken, cancellationToken);
        }
        else
        {
            var updateResult = Result.FirstFailureOrSuccess(
                refreshToken.UpdateValue(refreshTokenString),
                refreshToken.UpdateExpiresAt(expiresAt)
            );

            tokenRepository.Update(refreshToken);

            if (updateResult.IsFailure)
            {
                return Result.Failure<(string, Token)>(updateResult.Error);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success((jwtTokenString, refreshToken));
    }

    public async Task LogOutUserAsync(
        UserId userId,
        DeviceId deviceId,
        bool logOutAllDevices,
        CancellationToken cancellationToken
    )
    {
        if (logOutAllDevices)
        {
            var tokens = await tokenRepository.GetByUserIdAndTypeAsync(
                userId,
                TokenType.RefreshToken,
                cancellationToken
            );

            foreach (var t in tokens)
            {
                tokenRepository.Remove(t);
            }

            return;
        }

        var token = (
            await tokenRepository.GetByUserIdAndTypeAsync(
                userId,
                TokenType.RefreshToken,
                cancellationToken
            )
        ).Find(t => t.DeviceId == deviceId);

        if (token is null)
        {
            return;
        }

        tokenRepository.Remove(token);
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

        var uriBuilder = new UriBuilder(
            $"{clientAppOptions.Value.BaseUrl}/{clientAppOptions.Value.EmailVerificationPath}"
        );

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
        Token? emailVerificationToken = (
            await tokenRepository.GetByUserIdAndTypeAsync(
                userId,
                TokenType.EmailVerificationToken,
                cancellationToken
            )
        )
            .OrderBy(t => t.CreatedOn)
            .LastOrDefault();

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

            await tokenRepository.AddAsync(emailVerificationToken, cancellationToken);

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
