using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

public sealed record DailyNutritionOverviewReadModel(
    DailyNutritionOverviewId Id,
    UserId UserId,
    DateOnly Date,
    float CaloriesGoal,
    float CarbohydratesGoal,
    float FatGoal,
    float ProteinGoal
) : IReadModel<DailyNutritionOverviewId>
{
    public required NutritionalContent NutritionalContent { get; init; }
}
