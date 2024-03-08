using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrackYourLifeDotnet.Infrastructure.Options;

namespace TrackYourLifeDotnet.Infrastructure.OptionsSetup;

public class JwtBearerOptionsSetup : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions;

    public JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)
            ),
            // ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                // Add your custom validation here. For example:
                var jwtToken = context.SecurityToken as JwtSecurityToken;
                if (
                    jwtToken == null
                    || !jwtToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
                {
                    // Raise an exception if the token is not HMAC SHA 256
                    throw new SecurityTokenException("Invalid token");
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                // var jwtToken = context. as JwtSecurityToken;

                return Task.CompletedTask;
            }
        };
    }
}
