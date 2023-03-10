using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.Errors;

public static class DomainErrors
{
    public static class WeatherForecast
    {
        public static readonly Error Empty =
            new("WeatherForecast.Empty", "Weather forecast is empty");

        public static readonly Error NotFound =
            new("WeatherForecast.NotFound", "Weather forecast not found");
    }

    public static class User
    {
        public static readonly Error EmailAlreadyUsed =
            new("User.EmailAlreadyInUse", "The specified email is already used.");

        public static readonly Func<Guid, Error> NotFound = id =>
            new Error("User.NotFound", $"The user with the identifier {id} was not found.");

        public static readonly Error InvalidCredentials =
            new("User.InvalidCredentials", "The provided credentials are invalid");

        public static readonly Error InvalidRefreshToken =
            new("User.InvalidRefreshToken", "The provided refresh token is invalid");

        public static readonly Error ExpiredRefreshToken =
            new("User.ExpiredRefreshToken", "The provided refresh token is expired");

        public static readonly Error InvalidJwtToken =
            new("User.InvalidJwtToken", "The provided JWT token is invalid");
    }

    // public static class Gathering
    // {
    //     public static readonly Func<Guid, Error> NotFound = id =>
    //         new Error(
    //             "Gathering.NotFound",
    //             $"The gathering with the identifier {id} was not found."
    //         );

    //     public static readonly Error InvitingCreator =
    //         new("Gathering.InvitingCreator", "Can't send invitation to the gathering creator");

    //     public static readonly Error AlreadyPassed =
    //         new("Gathering.AlreadyPassed", "Can't send invitation for gathering in the past");

    //     public static readonly Error Expired =
    //         new("Gathering.Expired", "Can't accept invitation for expired gathering");
    // }

    public static class Email
    {
        public static readonly Error Empty = new("Email.Empty", "Email is empty");

        public static readonly Error TooLong = new("Email.TooLong", "Email is too long");

        public static readonly Error InvalidFormat =
            new("Email.InvalidFormat", "Email format is invalid");
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

    public static class FirstName
    {
        public static readonly Error Empty = new("FirstName.Empty", "First name is empty");

        public static readonly Error TooLong =
            new("LastName.TooLong", "FirstName name is too long");
    }

    public static class LastName
    {
        public static readonly Error Empty = new("LastName.Empty", "Last name is empty");

        public static readonly Error TooLong = new("LastName.TooLong", "Last name is too long");
    }
}
