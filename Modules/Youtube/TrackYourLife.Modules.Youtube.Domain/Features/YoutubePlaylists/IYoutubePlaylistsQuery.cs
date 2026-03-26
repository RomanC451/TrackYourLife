using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

public interface IYoutubePlaylistsQuery
{
    Task<IReadOnlyList<YoutubePlaylistReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<YoutubePlaylistReadModel?> GetByIdAndUserIdAsync(
        YoutubePlaylistId id,
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<YoutubePlaylistVideoReadModel>> GetVideosByPlaylistIdOrderedAsync(
        YoutubePlaylistId playlistId,
        CancellationToken cancellationToken = default
    );
}
