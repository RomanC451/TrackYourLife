using System.Reflection;
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
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(TrackYourLifeDotnet.Application.AssemblyReference.Assembly);
        services.AddFeatureManagement(configuration.GetSection("FeatureFlags"));

        return services;
    }
}
