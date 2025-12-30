using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

public static class YoutubeChannelsErrors
{
    public static readonly Func<YoutubeChannelId, Error> NotFound = id =>
        Error.NotFound(id, nameof(YoutubeChannel));

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
