using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.SharedLib.Application.Behaviors;

namespace TrackYourLife.Modules.Payments.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddPaymentsApplicationServices(
        this IServiceCollection services
    )
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);
            cfg.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
