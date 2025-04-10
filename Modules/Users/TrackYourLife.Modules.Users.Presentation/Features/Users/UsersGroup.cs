namespace TrackYourLife.Modules.Users.Presentation.Features.Users;

internal sealed class UsersGroup : Group
{
    public UsersGroup()
    {
        Configure(
            ApiRoutes.Users,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
