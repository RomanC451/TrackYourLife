using Mapster;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Core.Mapper;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Utils;

public static class NutritionMapperHelper
{
    public static INutritionMapper CreateMapper()
    {
        var nutrionModuleConfig = new TypeAdapterConfig();
        nutrionModuleConfig.Scan(AssemblyReference.Assembly);

        var serviceProvider = new ServiceCollection()
            .AddSingleton(nutrionModuleConfig)
            .BuildServiceProvider();

        return new NutritionMapper(serviceProvider, nutrionModuleConfig);
    }
}
