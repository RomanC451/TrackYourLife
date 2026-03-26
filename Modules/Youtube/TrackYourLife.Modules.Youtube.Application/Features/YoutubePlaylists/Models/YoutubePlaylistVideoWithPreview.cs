using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Models;

public sealed record YoutubePlaylistVideoWithPreview(
    YoutubePlaylistVideoReadModel Video,
    YoutubeVideoPreview? Preview
);
