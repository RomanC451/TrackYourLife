using Mapster;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.DailyNutritionOverviews;

/// <summary>
/// Represents the configuration for mapping between different types related to food diaries.
/// </summary>
public sealed class DailyNutritionOverviewMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //Read models to DTOs
        config.NewConfig<DailyNutritionOverviewReadModel, DailyNutritionOverviewDto>();
    }
}
