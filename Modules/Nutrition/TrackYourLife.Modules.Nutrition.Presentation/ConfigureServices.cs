using Microsoft.Extensions.DependencyInjection;

namespace TrackYourLife.Modules.Nutrition.Presentation;

public static class ConfigureServices
{
    public static IServiceCollection AddNutritionPresentationServices(
        this IServiceCollection services
    )
    {
        return services;
    }
}
