using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.GuitarTabs.Application.Core.Behaviors;
using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Application.Behaviors;

namespace TrackYourLife.Modules.GuitarTabs.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddGuitarTabsApplicationServices(
        this IServiceCollection services
    )
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);
        services.AddScoped<IGuitarTabLibrary, GuitarTabLibrary>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);
            cfg.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(GuitarTabsUnitOfWorkBehavior<,>));
        });

        return services;
    }
}
