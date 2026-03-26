using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Models;

internal sealed record YoutubePlaylistVideoItemDto(
    Guid Id,
    string YoutubeId,
    DateTime AddedOnUtc,
    YoutubeVideoPreview? VideoPreview
);

internal sealed record YoutubePlaylistDetailDto(
    Guid Id,
    string Name,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc,
    IReadOnlyList<YoutubePlaylistVideoItemDto> Videos
);

internal static class YoutubePlaylistMappingExtensions
{
    public static YoutubePlaylistVideoItemDto ToVideoItemDto(this YoutubePlaylistVideoWithPreview item)
    {
        return new YoutubePlaylistVideoItemDto(
            item.Video.Id.Value,
            item.Video.YoutubeId,
            item.Video.AddedOnUtc,
            item.Preview
        );
    }

    public static YoutubePlaylistDetailDto ToDetailDto(this GetPlaylistByIdResult result)
    {
        return new YoutubePlaylistDetailDto(
            result.Playlist.Id.Value,
            result.Playlist.Name,
            result.Playlist.CreatedOnUtc,
            result.Playlist.ModifiedOnUtc,
            result.Videos.Select(v => v.ToVideoItemDto()).ToList()
        );
    }
}
