namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes;

internal sealed class RecipesGroup : Group
{
    public RecipesGroup()
    {
        Configure(
            ApiRoutes.Recipes,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
