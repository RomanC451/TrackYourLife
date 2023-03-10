using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scrutor;
using TrackYourLifeDotnet.Infrastructure.OptionsSetup;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.Scan(
            selector =>
                selector
                    .FromAssemblies(TrackYourLifeDotnet.Infrastructure.AssemblyReference.Assembly)
                    .AddClasses(false)
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
        );

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
