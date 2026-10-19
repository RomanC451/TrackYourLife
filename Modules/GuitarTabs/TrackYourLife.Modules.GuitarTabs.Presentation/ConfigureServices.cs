using Microsoft.Extensions.DependencyInjection;

namespace TrackYourLife.Modules.GuitarTabs.Presentation;

public static class ConfigureServices
{
    public static IServiceCollection AddGuitarTabsPresentationServices(
        this IServiceCollection services
    )
    {
        return services;
    }
}
