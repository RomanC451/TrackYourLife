namespace TrackYourLife.Modules.Users.Presentation.Features.PersonalAccessTokens;

internal sealed class PersonalAccessTokensGroup : Group
{
    public PersonalAccessTokensGroup()
    {
        Configure(
            ApiRoutes.PersonalAccessTokens,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
