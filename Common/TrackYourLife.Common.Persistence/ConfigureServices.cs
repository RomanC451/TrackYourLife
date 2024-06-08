using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Persistence.Repositories;

namespace TrackYourLife.Common.Persistence;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationWriteDbContext>();
        services.AddDbContext<ApplicationReadDbContext>();

        services.Scan(
            selector =>
                selector
                    .FromAssemblies(AssemblyReference.Assembly)
                    .AddClasses(
                        classes =>
                            classes.Where(
                                type => !typeof(ICachedRepository).IsAssignableFrom(type)
                            ),
                        false
                    )
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
        );

        services.Decorate<IFoodRepository, CachedFoodRepository>();

        return services;
    }
}
