using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Features.Users;

public static class UserErrors
{
    public static readonly Func<UserId, Error> NotFound = id => Error.NotFound(id, nameof(User));

    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided credentials are invalid"
    );

    public static class Email
    {
        public static readonly Error Empty = new("Email.Empty", "Email is empty");

        public static readonly Error TooLong = new("Email.TooLong", "Email is too long");

        public static readonly Error InvalidFormat = new(
            "Email.InvalidFormat",
            "Email format is invalid"
        );

        public static readonly Error AlreadyUsed = new(
            "Email.AlreadyInUse",
            "The specified email is already used."
        );

        public static readonly Error NotVerified = new(
            "Email.NotVerified",
            "The specified email is not verified."
        );

        public static readonly Error EmailNotFound = new(
            "Email.NotFound",
            "The specified email was not found."
        );

        public static readonly Error AlreadyVerified = new(
            "Email.AlreadyVerified",
            "The specified email is already verified."
        );
    }

    public static class Password
    {
        public static readonly Error Empty = new("Password.Empty", "Password is empty");

        public static readonly Error TooShort = new("Password.TooShort", "Password is too short");

        public static readonly Error UpperCase = new(
            "Password.UpperCase",
            "Password must have at least one upper case letter"
        );

        public static readonly Error LowerCase = new(
            "Password.LowerCase",
            "Password must have at least one lower case letter"
        );

        public static readonly Error Number = new(
            "Password.Number",
            "Password must have at least one number"
        );

        public static readonly Error SpecialCharacter = new(
            "Password.Symbol",
            "Password must have at least one special character"
        );
    }

    public static class Name
    {
        public static readonly Error Empty = new("Name.Empty", "Name is empty");

        public static readonly Error TooLong = new("Name.TooLong", "Name is too long");
    }

    public static class EmailVerificationToken
    {
        public static readonly Error Invalid = new(
            "VerificationToken.Invalid",
            "The provided verification token is invalid"
        );
    }
}
