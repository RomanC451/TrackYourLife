namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

public record YoutubeVideoPreview(
    string VideoId,
    string Title,
    string ThumbnailUrl,
    string ChannelName,
    string ChannelId,
    DateTime PublishedAt,
    string Duration,
    long ViewCount
);

