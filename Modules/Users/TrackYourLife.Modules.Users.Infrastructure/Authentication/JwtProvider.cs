using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Infrastructure.Options;

namespace TrackYourLife.Modules.Users.Infrastructure.Authentication;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string Generate(User user)
    {
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value)
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256
        );

        var exp = DateTime.UtcNow.AddMinutes(_options.MinutesToExpire);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddMinutes(_options.MinutesToExpire),
            signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
