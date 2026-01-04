using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;

public sealed record YoutubeSettingsDto(
    int MaxEntertainmentVideosPerDay,
    SettingsChangeFrequency SettingsChangeFrequency,
    int? DaysBetweenChanges,
    DateTime? LastSettingsChangeUtc,
    DayOfWeek? SpecificDayOfWeek,
    int? SpecificDayOfMonth
);

internal static class YoutubeSettingsDtoExtensions
{
    public static YoutubeSettingsDto ToDto(this YoutubeSetting settings)
    {
        return new YoutubeSettingsDto(
            MaxEntertainmentVideosPerDay: settings.MaxEntertainmentVideosPerDay,
            SettingsChangeFrequency: settings.SettingsChangeFrequency,
            DaysBetweenChanges: settings.DaysBetweenChanges,
            LastSettingsChangeUtc: settings.LastSettingsChangeUtc,
            SpecificDayOfWeek: settings.SpecificDayOfWeek,
            SpecificDayOfMonth: settings.SpecificDayOfMonth
        );
    }

    public static YoutubeSettingsDto ToDto(this YoutubeSettingReadModel settings)
    {
        return new YoutubeSettingsDto(
            MaxEntertainmentVideosPerDay: settings.MaxEntertainmentVideosPerDay,
            SettingsChangeFrequency: settings.SettingsChangeFrequency,
            DaysBetweenChanges: settings.DaysBetweenChanges,
            LastSettingsChangeUtc: settings.LastSettingsChangeUtc,
            SpecificDayOfWeek: settings.SpecificDayOfWeek,
            SpecificDayOfMonth: settings.SpecificDayOfMonth
        );
    }
}
