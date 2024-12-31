using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Nutrition.Application.Core.Behaviors;
using TrackYourLife.SharedLib.Application.Behaviors;

namespace TrackYourLife.Modules.Nutrition.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddNutritionApplicationServices(
        this IServiceCollection services
    )
    {
        // Add validators from the assembly
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        // Add MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);

            cfg.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));

            cfg.AddOpenBehavior(typeof(NutritionUnitOfWorkBehavior<,>));
        });

        return services;
    }
}
