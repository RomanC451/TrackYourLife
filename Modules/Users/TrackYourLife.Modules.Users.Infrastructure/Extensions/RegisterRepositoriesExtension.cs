using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Infrastructure.Data;
using TrackYourLife.SharedLib.Domain.Repositories;

namespace TrackYourLife.Modules.Users.Infrastructure.Extensions;

internal static class RegisterRepositoriesExtension
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.Scan(selector =>
            selector
                .FromAssemblies(AssemblyReference.Assembly)
                .AddClasses(
                    classes =>
                        classes.Where(type =>
                            !typeof(ICachedRepository).IsAssignableFrom(type)
                            && (type.Name.EndsWith("Repository") || type.Name.EndsWith("Query"))
                        ),
                    false
                )
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();

        return services;
    }
}
