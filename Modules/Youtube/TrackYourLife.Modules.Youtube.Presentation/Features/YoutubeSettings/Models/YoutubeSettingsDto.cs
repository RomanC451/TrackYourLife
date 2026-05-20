using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;

public sealed record YoutubeCategorySettingsDto(
    Guid Id,
    string Name,
    int MaxVideosPerDay,
    int DisplayOrder,
    int SubscribedChannelCount
);

public sealed record YoutubeSettingsDto(
    IReadOnlyList<YoutubeCategorySettingsDto> Categories,
    SettingsChangeFrequency? SettingsChangeFrequency,
    int? DaysBetweenChanges,
    DateTime? LastSettingsChangeUtc,
    DayOfWeek? SpecificDayOfWeek,
    int? SpecificDayOfMonth
);

internal static class YoutubeSettingsDtoExtensions
{
    public static YoutubeSettingsDto ToDto(this YoutubePolicyReadModel model)
    {
        var counts = model.SubscribedChannelCountsByCategoryId;

        var categories = model
            .Categories.Select(c => new YoutubeCategorySettingsDto(
                    c.Id.Value,
                    c.Name,
                    c.MaxVideosPerDay,
                    c.DisplayOrder,
                    counts is not null && counts.TryGetValue(c.Id, out var n) ? n : 0
                ))
            .ToList();

        if (model.Settings is null)
        {
            return new YoutubeSettingsDto(
                Categories: categories,
                SettingsChangeFrequency: null,
                DaysBetweenChanges: null,
                LastSettingsChangeUtc: null,
                SpecificDayOfWeek: null,
                SpecificDayOfMonth: null
            );
        }

        var s = model.Settings;
        return new YoutubeSettingsDto(
            Categories: categories,
            SettingsChangeFrequency: s.SettingsChangeFrequency,
            DaysBetweenChanges: s.DaysBetweenChanges,
            LastSettingsChangeUtc: s.LastSettingsChangeUtc,
            SpecificDayOfWeek: s.SpecificDayOfWeek,
            SpecificDayOfMonth: s.SpecificDayOfMonth
        );
    }
}
