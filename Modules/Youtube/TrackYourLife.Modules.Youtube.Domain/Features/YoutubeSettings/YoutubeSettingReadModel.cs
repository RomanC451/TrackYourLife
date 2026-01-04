using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

public sealed record YoutubeSettingReadModel(
    YoutubeSettingsId Id,
    UserId UserId,
    int MaxEntertainmentVideosPerDay,
    SettingsChangeFrequency SettingsChangeFrequency,
    int? DaysBetweenChanges,
    DateTime? LastSettingsChangeUtc,
    DayOfWeek? SpecificDayOfWeek,
    int? SpecificDayOfMonth,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<YoutubeSettingsId>;
