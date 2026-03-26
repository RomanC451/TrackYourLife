using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

public static class YoutubePlaylistErrors
{
    public static readonly Func<Guid, Error> NotFound = playlistId =>
        new(
            "YoutubePlaylists.NotFound",
            $"Playlist with id '{playlistId}' was not found.",
            404
        );

    public static readonly Func<string, Error> VideoAlreadyInPlaylist = youtubeId =>
        new(
            "YoutubePlaylists.VideoAlreadyInPlaylist",
            $"YouTube video '{youtubeId}' is already in this playlist.",
            409
        );

    public static readonly Func<string, Error> VideoNotInPlaylist = youtubeId =>
        new(
            "YoutubePlaylists.VideoNotInPlaylist",
            $"YouTube video '{youtubeId}' is not in this playlist.",
            404
        );
}
