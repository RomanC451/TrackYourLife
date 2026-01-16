using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;

public interface IWatchedVideosRepository
{
    Task<WatchedVideo?> GetByUserIdAndVideoIdAsync(
        UserId userId,
        string videoId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<WatchedVideo>> GetByUserIdAndVideoIdsAsync(
        UserId userId,
        IEnumerable<string> videoIds,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(WatchedVideo watchedVideo, CancellationToken cancellationToken = default);
}
