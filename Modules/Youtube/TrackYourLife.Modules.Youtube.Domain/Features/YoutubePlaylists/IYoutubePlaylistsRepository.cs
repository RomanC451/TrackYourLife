using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

public interface IYoutubePlaylistsRepository
{
    Task<YoutubePlaylist?> GetByIdAsync(
        YoutubePlaylistId id,
        CancellationToken cancellationToken = default
    );

    Task<YoutubePlaylist?> GetByIdAndUserIdAsync(
        YoutubePlaylistId id,
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(YoutubePlaylist playlist, CancellationToken cancellationToken = default);

    void Remove(YoutubePlaylist playlist);

    void Update(YoutubePlaylist playlist);
}
