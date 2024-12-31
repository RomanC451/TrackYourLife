using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Domain.Tokens;

public static class TokenErrors
{
    public static class JwtToken
    {
        public static readonly Func<TokenId, Error> NotFound = id =>
            Error.NotFound(id, nameof(JwtToken));
        public static readonly Error AlreadyExists =
            new("JwtToken.AlreadyExists", "The provided jwt token already exists");
        public static readonly Error Invalid =
            new("JwtToken.Invalid", "The provided jwt token is invalid");

        public static readonly Error Expired =
            new("JwtToken.Expired", "The provided jwt token is expired");
    }

    public static class RefreshToken
    {
        public static readonly Func<TokenId, Error> NotFound = id =>
            Error.NotFound(id, nameof(RefreshToken));

        public static readonly Error NotExisting = new Error(
            "RefreshToken.NotFound",
            $"The refresh token doesn't exist in the sent request."
        );
        public static readonly Error AlreadyExists =
            new("RefreshToken.AlreadyExists", "The provided refresh token already exists");
        public static readonly Error Invalid =
            new("RefreshToken.Invalid", "The provided refresh token is invalid");

        public static readonly Error Expired =
            new("RefreshToken.Expired", "The provided refresh token is expired");
    }
}
