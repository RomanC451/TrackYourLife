using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;

internal sealed record UpdateYoutubeSettingsRequest(
    int MaxDivertissmentVideosPerDay,
    SettingsChangeFrequency SettingsChangeFrequency,
    int? DaysBetweenChanges,
    DayOfWeek? SpecificDayOfWeek,
    int? SpecificDayOfMonth
);
