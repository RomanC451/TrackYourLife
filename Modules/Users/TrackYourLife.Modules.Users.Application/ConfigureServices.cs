using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Users.Application.Core;
using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Core.Behaviors;
using TrackYourLife.SharedLib.Application.Behaviors;

namespace TrackYourLife.Modules.Users.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddUsersApplicationServices(this IServiceCollection services)
    {
        // Add validators from the assembly
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        // Add Mapster
        var usersModuleConfig = new TypeAdapterConfig();
        usersModuleConfig.Scan(AssemblyReference.Assembly);
        services.AddSingleton(usersModuleConfig);
        services.AddScoped<IUsersMapper, UsersMapper>();

        // Add MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);

            cfg.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(UsersUnitOfWorkBehavior<,>));
        });

        return services;
    }
}
