using Mapster;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Core.Mapper;

namespace TrackYourLife.Modules.Nutrition.Presentation;

public static class ConfigureServices
{
    public static IServiceCollection AddNutritionPresentationServices(
        this IServiceCollection services
    )
    {
        // Add Mapster
        var nutritionModuleConfig = new TypeAdapterConfig();
        nutritionModuleConfig.Scan(AssemblyReference.Assembly);
        services.AddSingleton(nutritionModuleConfig);
        services.AddScoped<INutritionMapper>(serviceProvider => new NutritionMapper(
            serviceProvider,
            nutritionModuleConfig
        ));

        return services;
    }
}
