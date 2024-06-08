using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Common.Infrastructure.FluentValidation;

namespace TrackYourLife.Common.Infrastructure.Options;

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
