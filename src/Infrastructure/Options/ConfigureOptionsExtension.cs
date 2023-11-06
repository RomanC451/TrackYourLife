using Microsoft.Extensions.DependencyInjection;
using TrackYourLifeDotnet.Infrastructure.FluentValidation;

namespace TrackYourLifeDotnet.Infrastructure.Options;

public static class ConfigureOptionsExtension
{
    public static IServiceCollection AddOptionsWithFluentValidation<TOptions>(this IServiceCollection services, string configurationSection) where TOptions: class
    {
        services
            .AddOptions<TOptions>()
            .BindConfiguration(configurationSection)
            .ValidateFluentValidation()
            .ValidateOnStart();

        return services;
    }

    
}