
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.SharedLib.Infrastructure.FluentValidation;

namespace TrackYourLife.SharedLib.Infrastructure.Extensions;

public static class ConfigureOptionsExtension
{
    public static IServiceCollection AddOptionsWithFluentValidation<TOptions>(
        this IServiceCollection services,
        string configurationSection
    )
        where TOptions : class
    {
        services
            .AddOptions<TOptions>()
            .BindConfiguration(configurationSection)
            .ValidateFluentValidation()
            .ValidateOnStart();

        return services;
    }
}
