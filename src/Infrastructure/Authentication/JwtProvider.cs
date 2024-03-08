using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using Microsoft.Extensions.Options;
using TrackYourLifeDotnet.Infrastructure.Options;
using TrackYourLifeDotnet.Domain.Users;

namespace TrackYourLifeDotnet.Infrastructure.Authentication;

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
