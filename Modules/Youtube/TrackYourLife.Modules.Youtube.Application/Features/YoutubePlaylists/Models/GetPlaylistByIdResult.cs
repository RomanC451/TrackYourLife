using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Models;

public sealed record GetPlaylistByIdResult(
    YoutubePlaylistReadModel Playlist,
    IReadOnlyList<YoutubePlaylistVideoWithPreview> Videos
);
