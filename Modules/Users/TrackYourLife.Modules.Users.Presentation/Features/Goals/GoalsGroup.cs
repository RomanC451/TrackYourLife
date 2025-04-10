namespace TrackYourLife.Modules.Users.Presentation.Features.Goals;

internal sealed class GoalsGroup : Group
{
    public GoalsGroup()
    {
        Configure(
            ApiRoutes.Goals,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
