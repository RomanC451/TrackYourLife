namespace TrackYourLife.Modules.Nutrition.Presentation.Features.DailyNutritionOverviews;

internal class DailyNutritionOverviewsGroup : Group
{
    public DailyNutritionOverviewsGroup()
    {
        Configure(
            ApiRoutes.DailyNutritionOverviews,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
