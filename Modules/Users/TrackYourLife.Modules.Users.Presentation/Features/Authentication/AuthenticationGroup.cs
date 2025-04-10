namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication;

internal sealed class AuthenticationGroup : Group
{
    public AuthenticationGroup()
    {
        Configure(ApiRoutes.Authentication, ep => { });
    }
}
