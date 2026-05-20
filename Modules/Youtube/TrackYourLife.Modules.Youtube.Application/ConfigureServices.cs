using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Youtube.Application.Core.Behaviors;
using TrackYourLife.SharedLib.Application.Behaviors;

namespace TrackYourLife.Modules.Youtube.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddYoutubeApplicationServices(this IServiceCollection services)
    {
        // Internal validators (e.g. YoutubeModuleOptionsValidator) require includeInternalTypes
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

        // Add MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);

            cfg.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));

            cfg.AddOpenBehavior(typeof(YoutubeUnitOfWorkBehavior<,>));
        });

        return services;
    }
}
