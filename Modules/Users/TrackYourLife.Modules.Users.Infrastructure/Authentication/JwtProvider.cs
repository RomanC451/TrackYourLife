using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.SharedLib.Application.Permissions;

namespace TrackYourLife.Modules.Users.Infrastructure.Authentication;

internal sealed class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public Task<string> GenerateAsync(
        UserReadModel user,
        CancellationToken cancellationToken = default
    )
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
        };

        if (
            user.PlanType == PlanType.Pro
            && (
                !user.SubscriptionEndsAtUtc.HasValue || user.SubscriptionEndsAtUtc > DateTime.UtcNow
            )
        )
        {
            claims.Add(new Claim("permissions", Allow.Pro));
        }

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims.ToArray(),
            null,
            DateTime.UtcNow.AddMinutes(_options.MinutesToExpire),
            signingCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Task.FromResult(tokenString);
    }
}
