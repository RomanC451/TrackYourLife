using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

public static class YoutubeSettingsErrors
{
    public static readonly Error InvalidFrequency = new(
        "Youtube.Settings.InvalidFrequency",
        "Invalid settings change frequency.",
        400
    );

    public static readonly Error InvalidFrequencyConfiguration = new(
        "Youtube.Settings.InvalidFrequencyConfiguration",
        "Frequency configuration is invalid or missing required fields.",
        400
    );

    public static readonly Error InvalidDaysBetweenChanges = new(
        "Youtube.Settings.InvalidDaysBetweenChanges",
        "Days between changes must be greater than 0.",
        400
    );

    public static readonly Error SpecificDayOfWeekRequired = new(
        "Youtube.Settings.SpecificDayOfWeekRequired",
        "Specific day of week is required for this frequency type.",
        400
    );

    public static readonly Error InvalidDayOfMonth = new(
        "Youtube.Settings.InvalidDayOfMonth",
        "Day of month must be between 1 and 31.",
        400
    );

    public static readonly Func<DateTime, Error> SettingsChangeNotAllowed = nextAllowedChange =>
        new(
            "Youtube.SettingsChangeNotAllowed",
            $"Settings can only be changed after {nextAllowedChange:yyyy-MM-dd HH:mm:ss} UTC.",
            403
        );

    public static readonly Func<DayOfWeek, Error> SettingsChangeNotAllowedForDayOfWeek =
        dayOfWeek =>
            new(
                "Youtube.SettingsChangeNotAllowed",
                $"Settings can only be changed on {dayOfWeek}.",
                403
            );

    public static readonly Func<int, Error> SettingsChangeNotAllowedForDayOfMonth = dayOfMonth =>
        new(
            "Youtube.SettingsChangeNotAllowed",
            $"Settings can only be changed on day {dayOfMonth} of the month.",
            403
        );

    public static readonly Error DivertissmentLimitReached = new(
        "Youtube.DivertissmentLimitReached",
        "Daily limit for divertissment videos has been reached.",
        403
    );
}
