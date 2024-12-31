using Mapster;
using MapsterMapper;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Core.Mapper;

/// <summary>
/// Represents a mapper for the Nutrition module in the TrackYourLife application.
/// </summary>
public class NutritionMapper(IServiceProvider serviceProvider, TypeAdapterConfig config)
    : ServiceMapper(serviceProvider, config),
        INutritionMapper;
