using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(TrackYourLifeDotnet.Application.AssemblyReference.Assembly);
        services.AddSingleton(config);

        services.AddScoped<IMapper, ServiceMapper>();

        services.AddMediatR(TrackYourLifeDotnet.Application.AssemblyReference.Assembly);
        services.AddFeatureManagement(configuration.GetSection("FeatureFlags"));

        return services;
    }
}
