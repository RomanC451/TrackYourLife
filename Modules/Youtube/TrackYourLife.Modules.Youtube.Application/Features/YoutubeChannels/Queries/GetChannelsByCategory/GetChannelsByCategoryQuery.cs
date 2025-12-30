using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;

public sealed record GetChannelsByCategoryQuery(VideoCategory? Category)
    : IQuery<IEnumerable<YoutubeChannelReadModel>>;

