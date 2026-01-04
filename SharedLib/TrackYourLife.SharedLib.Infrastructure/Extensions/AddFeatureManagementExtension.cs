using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrackYourLife.SharedLib.Infrastructure.Extensions;

public static class AddFeatureManagementExtension
{
    public static IServiceCollection AddFeatureManagement<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string? configurationSectionName = null
    )
        where T : class, new()
    {
        var featureManagementConfig = new T();
        configuration
            .GetSection(configurationSectionName ?? "FeatureFlags")
            .Bind(featureManagementConfig);
        services.AddSingleton(featureManagementConfig);

        return services;
    }
}
