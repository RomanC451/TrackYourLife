using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.Errors;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Func<Guid, Error> NotFound = id =>
            new Error("User.NotFound", $"The user with the identifier {id} was not found.");

        public static readonly Error InvalidCredentials =
            new("User.InvalidCredentials", "The provided credentials are invalid");
    }

    public static class Email
    {
        public static readonly Error Empty = new("Email.Empty", "Email is empty");

        public static readonly Error TooLong = new("Email.TooLong", "Email is too long");

        public static readonly Error InvalidFormat =
            new("Email.InvalidFormat", "Email format is invalid");

        public static readonly Error AlreadyUsed =
            new("User.Email.AlreadyInUse", "The specified email is already used.");

        public static readonly Error NotVerified =
            new("User.Email.NotVerified", "The specified email is not verified.");
    }

    public static class Password
    {
        public static readonly Error Empty = new("Password.Empty", "Password is empty");

        public static readonly Error TooShort = new("Password.TooShort", "Password is too short");

        public static readonly Error UpperCase =
            new("Password.UpperCase", "Password must have at least one upper case letter");

        public static readonly Error LowerCase =
            new("Password.LowerCase", "Password must have at least one lower case letter");

        public static readonly Error Number =
            new("Password.Number", "Password must have at least one number");

        public static readonly Error SpecialCharacter =
            new("Password.Symbol", "Password must have at least one special character");
    }

    public static class Name
    {
        public static readonly Error Empty = new("Name.Empty", "Name is empty");

        public static readonly Error TooLong = new("Name.TooLong", "Name is too long");
    }

    public static class RefreshToken
    {
        public static readonly Func<Guid, Error> NotFound = id =>
            new Error(
                "RefreshToken.NotFound",
                $"The refresh token with the identifier {id} was not found."
            );
        public static readonly Error AlreadyExists =
            new("RefreshToken.AlreadyExists", "The provided refresh token already exists");
        public static readonly Error Invalid =
            new("RefreshToken.Invalid", "The provided refresh token is invalid");

        public static readonly Error Expired =
            new("RefreshToken.Expired", "The provided refresh token is expired");
    }

    public static class JwtToken
    {
        public static readonly Func<Guid, Error> NotFound = id =>
            new Error(
                "JwtToken.NotFound",
                $"The jwt token with the identifier {id} was not found."
            );
        public static readonly Error AlreadyExists =
            new("JwtToken.AlreadyExists", "The provided jwt token already exists");
        public static readonly Error Invalid =
            new("JwtToken.Invalid", "The provided jwt token is invalid");

        public static readonly Error Expired =
            new("JwtToken.Expired", "The provided jwt token is expired");
    }

    public static class EmailVerificationToken
    {
        public static readonly Error Invalid =
            new("VerificationToken.Invalid", "The provided verification token is invalid");
    }
}
