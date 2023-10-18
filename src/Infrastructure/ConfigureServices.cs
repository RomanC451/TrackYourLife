using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scrutor;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Infrastructure.OptionsSetup;
using TrackYourLifeDotnet.Infrastructure.Services;
using TrackYourLifeDotNet.Infrastructure.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.ConfigurationSection);
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        services.AddScoped<IAuthService, AuthService>();
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
