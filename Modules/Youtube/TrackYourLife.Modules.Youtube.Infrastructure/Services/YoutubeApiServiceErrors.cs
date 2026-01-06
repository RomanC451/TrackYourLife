using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Services;

public static class YoutubeApiServiceErrors
{
    public static readonly Func<string, Error> SearchChannelsFailed = message =>
        new("Youtube.SearchChannelsFailed", message, 500);

    public static readonly Func<string, Error> GetChannelVideosFailed = message =>
        new("Youtube.GetChannelVideosFailed", message, 500);

    public static readonly Func<string, Error> GetVideosFromChannelsFailed = message =>
        new("Youtube.GetVideosFromChannelsFailed", message, 500);

    public static readonly Func<string, Error> SearchVideosFailed = message =>
        new("Youtube.SearchVideosFailed", message, 500);

    public static readonly Func<string, Error> VideoNotFound = videoId =>
        new("Youtube.VideoNotFound", $"Video with ID '{videoId}' was not found.", 404);

    public static readonly Func<string, Error> GetVideoDetailsFailed = message =>
        new("Youtube.GetVideoDetailsFailed", message, 500);

    public static readonly Func<string, Error> ChannelNotFound = channelId =>
        new("Youtube.ChannelNotFound", $"Channel with ID '{channelId}' was not found.", 404);

    public static readonly Func<string, Error> GetChannelInfoFailed = message =>
        new("Youtube.GetChannelInfoFailed", message, 500);
}
