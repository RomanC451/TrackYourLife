using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

public static class PersonalAccessTokenErrors
{
    public static readonly Func<PersonalAccessTokenId, Error> NotFound = id =>
        Error.NotFound(id, nameof(PersonalAccessToken));

    public static readonly Func<PersonalAccessTokenId, Error> NotOwned = id =>
        Error.NotOwned(id, nameof(PersonalAccessToken));

    public static readonly Error AlreadyRevoked = new(
        "PersonalAccessToken.AlreadyRevoked",
        "This personal access token has already been revoked.",
        400
    );

    public static readonly Error InvalidToken = new(
        "PersonalAccessToken.Invalid",
        "The personal access token is invalid or has expired.",
        401
    );
}
