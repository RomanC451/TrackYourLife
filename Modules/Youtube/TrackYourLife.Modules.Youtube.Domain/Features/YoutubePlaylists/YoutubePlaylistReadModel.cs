using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

public sealed record YoutubePlaylistReadModel(
    YoutubePlaylistId Id,
    UserId UserId,
    string Name,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<YoutubePlaylistId>;
