namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Foods;

internal sealed class FoodsGroup : Group
{
    public FoodsGroup()
    {
        Configure(
            ApiRoutes.Foods,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
