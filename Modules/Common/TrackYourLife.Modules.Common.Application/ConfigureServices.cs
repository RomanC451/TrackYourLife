using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Common.Application.Core.Behaviors;
using TrackYourLife.SharedLib.Application.Behaviors;

namespace TrackYourLife.Modules.Common.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddCommonApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {

        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);

            cfg.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));

            cfg.AddOpenBehavior(typeof(CommonUnitOfWorkBehavior<,>));
        });



        return services;
    }
}
