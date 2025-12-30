namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Models;

public record YoutubeChannelSearchResult(
    string ChannelId,
    string Name,
    string Description,
    string ThumbnailUrl,
    long SubscriberCount
);

