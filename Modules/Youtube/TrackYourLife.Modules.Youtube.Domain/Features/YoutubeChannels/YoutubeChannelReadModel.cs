using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

public sealed record YoutubeChannelReadModel(
    YoutubeChannelId Id,
    UserId UserId,
    string YoutubeChannelId,
    string Name,
    string? ThumbnailUrl,
    YoutubeCategoryId YoutubeCategoryId,
    string CategoryName,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<YoutubeChannelId>;
