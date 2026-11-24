using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Application.Permissions;

namespace TrackYourLife.Modules.Users.Infrastructure.Authentication;

internal sealed class PersonalAccessTokenAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IPersonalAccessTokenRepository personalAccessTokenRepository,
    IPersonalAccessTokenHasher personalAccessTokenHasher,
    IUserQuery userQuery,
    IUsersUnitOfWork unitOfWork
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? authorization = Request.Headers.Authorization;

        if (
            string.IsNullOrWhiteSpace(authorization)
            || !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
        )
        {
            return AuthenticateResult.NoResult();
        }

        var rawToken = authorization["Bearer ".Length..].Trim();

        if (
            string.IsNullOrEmpty(rawToken)
            || !rawToken.StartsWith(PersonalAccessToken.TokenPrefixMarker, StringComparison.Ordinal)
        )
        {
            return AuthenticateResult.NoResult();
        }

        var tokenHash = personalAccessTokenHasher.Hash(rawToken);
        var utcNow = DateTime.UtcNow;

        var personalAccessToken = await personalAccessTokenRepository.GetActiveByTokenHashAsync(
            tokenHash,
            utcNow,
            Context.RequestAborted
        );

        if (personalAccessToken is null)
        {
            return AuthenticateResult.Fail(PersonalAccessTokenErrors.InvalidToken.Message);
        }

        var user = await userQuery.GetByIdAsync(personalAccessToken.UserId, Context.RequestAborted);

        if (user is null)
        {
            return AuthenticateResult.Fail(PersonalAccessTokenErrors.InvalidToken.Message);
        }

        personalAccessToken.MarkUsed(utcNow);
        personalAccessTokenRepository.Update(personalAccessToken);

        try
        {
            await unitOfWork.SaveChangesAsync(Context.RequestAborted);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to update personal access token last-used timestamp.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
        };

        if (
            user.PlanType == PlanType.Pro
            && (
                !user.SubscriptionEndsAtUtc.HasValue || user.SubscriptionEndsAtUtc > utcNow
            )
        )
        {
            claims.Add(new Claim("permissions", Allow.Pro));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
