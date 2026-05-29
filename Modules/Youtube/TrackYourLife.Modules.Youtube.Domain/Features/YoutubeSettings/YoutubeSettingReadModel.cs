using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

public sealed record YoutubeSettingReadModel(
    YoutubeSettingsId Id,
    UserId UserId,
    string? SettingsPasswordHash,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<YoutubeSettingsId>
{
    public bool HasSettingsPassword => !string.IsNullOrEmpty(SettingsPasswordHash);
}
