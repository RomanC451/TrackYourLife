using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Users.Application.Core;

namespace TrackYourLife.Modules.Users.Infrastructure.Extensions;

internal static class AddFeatureManagementExtension
{
    private const string ConfigurationSection = "UsersFeatureFlags";

    public static IServiceCollection AddFeatureManagement(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var featureManagementConfig = new UsersFeatureManagement();
        configuration.GetSection(ConfigurationSection).Bind(featureManagementConfig);
        services.AddSingleton(featureManagementConfig);

        return services;
    }
}
