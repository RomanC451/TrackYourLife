namespace TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries;

internal class FoodDiariesGroup : Group
{
    public FoodDiariesGroup()
    {
        Configure(
            ApiRoutes.FoodDiaries,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
