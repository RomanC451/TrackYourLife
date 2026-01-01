using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;

public static class WatchedVideoErrors
{
    public static readonly Func<string, Error> NotFound = videoId =>
        new(
            "Youtube.WatchedVideo.NotFound",
            $"Watched video with ID '{videoId}' was not found.",
            404
        );
}
