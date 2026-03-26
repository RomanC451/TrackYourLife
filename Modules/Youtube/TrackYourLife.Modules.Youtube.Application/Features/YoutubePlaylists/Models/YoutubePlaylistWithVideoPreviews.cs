using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Models;

public sealed record YoutubePlaylistWithVideoPreviews(
    Guid Id,
    string Name,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc,
    IReadOnlyList<YoutubeVideoPreview> VideoPreviews
);
