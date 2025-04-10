using Mapster;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.DailyNutritionOverviews;

/// <summary>
/// Represents the configuration for mapping between different types related to food diaries.
/// </summary>
internal sealed class DailyNutritionOverviewMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //Read models to DTOs
        config.NewConfig<DailyNutritionOverviewReadModel, DailyNutritionOverviewDto>();
    }
}
