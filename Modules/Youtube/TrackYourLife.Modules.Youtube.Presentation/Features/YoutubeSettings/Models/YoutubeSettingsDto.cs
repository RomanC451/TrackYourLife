using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;

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
    bool HasSettingsPassword
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

        var hasSettingsPassword = model.Settings?.HasSettingsPassword ?? false;

        return new YoutubeSettingsDto(Categories: categories, HasSettingsPassword: hasSettingsPassword);
    }
}
