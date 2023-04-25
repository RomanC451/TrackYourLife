using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TrackYourLifeDotnet.Infrastructure.Authentication;

namespace TrackYourLifeDotnet.Infrastructure.OptionsSetup;

public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private const string SectionName = "Jwt";

    private readonly IConfiguration _configuration;

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void Configure(JwtOptions options)
    {
        var section = _configuration.GetSection(SectionName);

        if (section?.Exists() != true)
        {
            throw new InvalidOperationException(
                $"Missing or invalid '{SectionName}' configuration section."
            );
        }

        var configOptions = section.Get<JwtOptions>();

        if (configOptions == null)
        {
            throw new InvalidOperationException($"Invalid '{SectionName}' configuration section.");
        }

        var properties = typeof(JwtOptions).GetProperties(
            BindingFlags.Public | BindingFlags.Instance
        );

        foreach (var property in properties)
        {
            var value = property.GetValue(configOptions);

            if (value is string && string.IsNullOrWhiteSpace(value.ToString()))
            {
                throw new InvalidOperationException(
                    $"Missing or invalid value for property '{property.Name}' in '{SectionName}' section."
                );
            }

            property.SetValue(options, value);
        }
    }
}
