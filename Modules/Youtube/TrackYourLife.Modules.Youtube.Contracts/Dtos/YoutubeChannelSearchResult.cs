namespace TrackYourLife.Modules.Youtube.Contracts.Dtos;

public sealed record YoutubeChannelSearchResult(
    string ChannelId,
    string Name,
    string Description,
    string ThumbnailUrl,
    long SubscriberCount,
    bool AlreadySubscribed
);
