using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Contracts.Dtos;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.SearchYoutubeChannels;

public sealed record SearchYoutubeChannelsQuery(string Query, int MaxResults = 10)
    : IQuery<IEnumerable<YoutubeChannelSearchResult>>;

