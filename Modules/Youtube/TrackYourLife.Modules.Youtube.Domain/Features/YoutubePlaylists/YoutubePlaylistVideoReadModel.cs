using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

public sealed record YoutubePlaylistVideoReadModel(
    YoutubePlaylistVideoId Id,
    YoutubePlaylistId YoutubePlaylistId,
    string YoutubeId,
    DateTime AddedOnUtc
) : IReadModel<YoutubePlaylistVideoId>;
