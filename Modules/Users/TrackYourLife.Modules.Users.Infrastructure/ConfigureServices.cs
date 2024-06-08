using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Common.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.Services;
using TrackYourLife.Modules.Users.Infrastructure.OptionsSetup;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

namespace TrackYourLife.Modules.Users.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services
    )
    {

        services.AddOptionsWithFluentValidation<JwtOptions>(JwtOptions.ConfigurationSection);
        services.AddOptionsWithFluentValidation<EmailOptions>(EmailOptions.ConfigurationSection);

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();



        return services;
    }
}
