using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.SearchYoutubeChannels;

internal sealed class SearchYoutubeChannelsQueryHandler(IYoutubeApiService youtubeApiService)
    : IQueryHandler<SearchYoutubeChannelsQuery, IEnumerable<YoutubeChannelSearchResult>>
{
    public async Task<Result<IEnumerable<YoutubeChannelSearchResult>>> Handle(
        SearchYoutubeChannelsQuery request,
        CancellationToken cancellationToken
    )
    {
        return await youtubeApiService.SearchChannelsAsync(
            request.Query,
            request.MaxResults,
            cancellationToken
        );
    }
}

