using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

public static class YoutubeSettingsErrors
{
    public static readonly Error PasswordNotSet = new(
        "Youtube.Settings.PasswordNotSet",
        "No settings password is configured.",
        400
    );

    public static readonly Error PasswordAlreadySet = new(
        "Youtube.Settings.PasswordAlreadySet",
        "A settings password is already configured.",
        400
    );

    public static readonly Error InvalidPassword = new(
        "Youtube.Settings.InvalidPassword",
        "The settings password is incorrect.",
        401
    );

    public static readonly Error CurrentPasswordRequired = new(
        "Youtube.Settings.CurrentPasswordRequired",
        "Current password is required.",
        400
    );

    public static readonly Error NewPasswordRequired = new(
        "Youtube.Settings.NewPasswordRequired",
        "New password is required.",
        400
    );

    public static readonly Error ResetEmailRateLimited = new(
        "Youtube.Settings.ResetEmailRateLimited",
        "A password reset email was sent recently. Please wait before requesting another.",
        400
    );

    public static readonly Error AccountEmailNotVerified = new(
        "Youtube.Settings.AccountEmailNotVerified",
        "Verify your account email before resetting the settings password.",
        400
    );

    public static readonly Error FailedToSendResetEmail = new(
        "Youtube.Settings.FailedToSendResetEmail",
        "Could not send the password reset email. Your settings password was not changed.",
        500
    );

    public static readonly Error UserNotFound = new(
        "Youtube.Settings.UserNotFound",
        "User account was not found.",
        404
    );

    public static class Password
    {
        public static readonly Error Empty = new(
            "Youtube.Settings.Password.Empty",
            "Password is empty"
        );

        public static readonly Error TooShort = new(
            "Youtube.Settings.Password.TooShort",
            "Password is too short"
        );

        public static readonly Error UpperCase = new(
            "Youtube.Settings.Password.UpperCase",
            "Password must have at least one upper case letter"
        );

        public static readonly Error LowerCase = new(
            "Youtube.Settings.Password.LowerCase",
            "Password must have at least one lower case letter"
        );

        public static readonly Error Number = new(
            "Youtube.Settings.Password.Number",
            "Password must have at least one number"
        );

        public static readonly Error SpecialCharacter = new(
            "Youtube.Settings.Password.Symbol",
            "Password must have at least one special character"
        );
    }
}
