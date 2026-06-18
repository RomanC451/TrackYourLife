using Microsoft.Extensions.DependencyInjection;

namespace TrackYourLife.Modules.Reading.Presentation;

public static class ConfigureServices
{
    public static IServiceCollection AddReadingPresentationServices(
        this IServiceCollection services
    )
    {
        return services;
    }
}
