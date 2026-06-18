using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Reading.Application.Core.Behaviors;
using TrackYourLife.SharedLib.Application.Behaviors;

namespace TrackYourLife.Modules.Reading.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddReadingApplicationServices(
        this IServiceCollection services
    )
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);
            cfg.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ReadingUnitOfWorkBehavior<,>));
        });

        services.AddScoped<Services.IReadingStatisticsService, Services.ReadingStatisticsService>();
        services.AddScoped<Abstraction.IReadingGoalProvider, Abstraction.ReadingGoalProvider>();

        return services;
    }
}
