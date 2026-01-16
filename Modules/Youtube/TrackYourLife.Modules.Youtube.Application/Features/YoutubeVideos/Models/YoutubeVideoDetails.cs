namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

public record YoutubeVideoDetails(
    string VideoId,
    string Title,
    string Description,
    string EmbedUrl,
    string ThumbnailUrl,
    string ChannelName,
    string ChannelId,
    DateTime PublishedAt,
    string Duration,
    long ViewCount,
    long LikeCount
);
