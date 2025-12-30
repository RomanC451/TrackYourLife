using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Models;

internal sealed record YoutubeChannelDto(
    Guid Id,
    string YoutubeChannelId,
    string Name,
    string? ThumbnailUrl,
    VideoCategory Category,
    DateTime CreatedOnUtc
);

internal static class YoutubeChannelMappingExtensions
{
    public static YoutubeChannelDto ToDto(this YoutubeChannelReadModel readModel)
    {
        return new YoutubeChannelDto(
            Id: readModel.Id.Value,
            YoutubeChannelId: readModel.YoutubeChannelId,
            Name: readModel.Name,
            ThumbnailUrl: readModel.ThumbnailUrl,
            Category: readModel.Category,
            CreatedOnUtc: readModel.CreatedOnUtc
        );
    }
}

