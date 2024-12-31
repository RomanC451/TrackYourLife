using TrackYourLife.Modules.Users.Presentation.Contracts;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication;

internal class AuthenticationGroup : Group
{
    public AuthenticationGroup()
    {
        Configure(ApiRoutes.Authentication, ep => { });
    }
}
