namespace TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries;

internal class RecipeDiariesGroup : Group
{
    public RecipeDiariesGroup()
    {
        Configure(
            ApiRoutes.RecipeDiaries,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
