using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using TrackYourLife.Common.Application.Core.Behaviors;

namespace TrackYourLife.Common.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddValidatorsFromAssembly(
            AssemblyReference.Assembly
        );

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(AssemblyReference.Assembly);
        services.AddSingleton(config);

        services.AddScoped<IMapper, ServiceMapper>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                AssemblyReference.Assembly
            );

            cfg.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddFeatureManagement(configuration.GetSection("FeatureFlags"));


        return services;
    }
}
