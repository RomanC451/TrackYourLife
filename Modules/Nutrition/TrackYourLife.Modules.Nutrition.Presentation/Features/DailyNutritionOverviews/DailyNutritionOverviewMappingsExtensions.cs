using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.DailyNutritionOverviews;

/// <summary>
/// Represents the extension class for mapping between different types related to daily nutrition overviews.
/// </summary>

internal static class DailyNutritionOverviewMappingsExtensions
{
    public static DailyNutritionOverviewDto ToDto(
        this DailyNutritionOverviewReadModel dailyNutritionOverview
    )
    {
        return new DailyNutritionOverviewDto(
            dailyNutritionOverview.Id,
            dailyNutritionOverview.Date,
            dailyNutritionOverview.NutritionalContent,
            dailyNutritionOverview.CaloriesGoal,
            dailyNutritionOverview.CarbohydratesGoal,
            dailyNutritionOverview.FatGoal,
            dailyNutritionOverview.ProteinGoal
        );
    }
}
