using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

public interface IYoutubeSettingsRepository
{
    Task<YoutubeSetting?> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(YoutubeSetting settings, CancellationToken cancellationToken = default);

    void Update(YoutubeSetting settings);
}
