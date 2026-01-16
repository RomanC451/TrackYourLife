using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

public static class YoutubeChannelsErrors
{
    public static readonly Func<string, Error> NotFound = youtubeChannelId =>
        new(
            "YoutubeChannels.NotFound",
            $"Youtube channel with YouTube ID '{youtubeChannelId}' was not found.",
            404
        );

    public static readonly Func<string, Error> NotFoundByYoutubeId = youtubeChannelId =>
        new(
            "YoutubeChannels.NotFoundByYoutubeId",
            $"Youtube channel with YouTube ID '{youtubeChannelId}' was not found.",
            404
        );

    public static readonly Func<string, Error> AlreadyExists = youtubeChannelId =>
        new(
            "YoutubeChannels.AlreadyExists",
            $"Youtube channel with YouTube ID '{youtubeChannelId}' already exists for this user.",
            409
        );
}
