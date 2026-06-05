namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

public record WatchedVideoHistoryEntry(
    YoutubeVideoPreview? Video,
    string VideoId,
    DateTime WatchedAtUtc
);
