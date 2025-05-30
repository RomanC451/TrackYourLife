namespace TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries;

internal sealed class NutritionDiariesGroup : Group
{
    public NutritionDiariesGroup()
    {
        Configure(
            ApiRoutes.NutritionDiaries,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
