using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using TrackYourLife.SharedLib.Domain.Repositories;

namespace TrackYourLife.SharedLib.Infrastructure.Extensions;

public static class RegisterRepositoriesExtension
{
    public static IServiceCollection RegisterRepositories(
        this IServiceCollection services,
        Assembly assembly,
        params Type[] ignoredTypes
    )
    {
        services.Scan(selector =>
            selector
                .FromAssemblies(assembly)
                .AddClasses(
                    classes =>
                        classes.Where(type =>
                            !ignoredTypes.Contains(type)
                            && !typeof(ICachedRepository).IsAssignableFrom(type)
                            && (type.Name.EndsWith("Repository") || type.Name.EndsWith("Query"))
                        ),
                    false
                )
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        return services;
    }
}
