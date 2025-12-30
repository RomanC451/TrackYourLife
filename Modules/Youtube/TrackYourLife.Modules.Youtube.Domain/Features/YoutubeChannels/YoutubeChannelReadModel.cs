using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

public sealed record YoutubeChannelReadModel(
    YoutubeChannelId Id,
    UserId UserId,
    string YoutubeChannelId,
    string Name,
    string? ThumbnailUrl,
    VideoCategory Category,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<YoutubeChannelId>;

