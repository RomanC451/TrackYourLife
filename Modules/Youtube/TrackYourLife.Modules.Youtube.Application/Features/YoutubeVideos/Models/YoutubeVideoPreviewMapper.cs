namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

internal static class YoutubeVideoPreviewMapper
{
    public static YoutubeVideoPreview FromDetails(YoutubeVideoDetails details, bool isWatched) =>
        new(
            details.VideoId,
            details.Title,
            details.ThumbnailUrl,
            details.ChannelName,
            details.ChannelId,
            details.PublishedAt,
            details.Duration,
            details.ViewCount,
            isWatched
        );
}
