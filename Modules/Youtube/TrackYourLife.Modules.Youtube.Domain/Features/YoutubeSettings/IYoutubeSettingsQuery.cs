using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

public interface IYoutubeSettingsQuery
{
    Task<YoutubeSettingReadModel?> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );
}
