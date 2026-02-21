using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Infrastructure.Options;
using TrackYourLife.Modules.Payments.Infrastructure.Services;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Payments.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddPaymentsInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

        services.AddOptionsWithFluentValidation<StripeOptions>(StripeOptions.ConfigurationSection);

        services.AddScoped<IStripeService, StripeService>();

        return services;
    }
}
