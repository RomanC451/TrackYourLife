namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

public interface IYoutubePlaylistVideosRepository
{
    Task<YoutubePlaylistVideo?> GetByPlaylistIdAndYoutubeIdAsync(
        YoutubePlaylistId playlistId,
        string youtubeId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsAsync(
        YoutubePlaylistId playlistId,
        string youtubeId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(YoutubePlaylistVideo item, CancellationToken cancellationToken = default);

    void Remove(YoutubePlaylistVideo item);
}
