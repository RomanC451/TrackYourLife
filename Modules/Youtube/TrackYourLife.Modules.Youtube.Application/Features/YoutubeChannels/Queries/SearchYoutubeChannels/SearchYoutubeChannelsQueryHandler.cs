using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Contracts.Dtos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.SearchYoutubeChannels;

internal sealed class SearchYoutubeChannelsQueryHandler(
    IYoutubeApiService youtubeApiService,
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeChannelsQuery youtubeChannelsQuery
) : IQueryHandler<SearchYoutubeChannelsQuery, IEnumerable<YoutubeChannelSearchResult>>
{
    public async Task<Result<IEnumerable<YoutubeChannelSearchResult>>> Handle(
        SearchYoutubeChannelsQuery request,
        CancellationToken cancellationToken
    )
    {
        var searchResult = await youtubeApiService.SearchChannelsAsync(
            request.Query,
            request.MaxResults,
            cancellationToken
        );

        if (searchResult.IsFailure)
        {
            return Result.Failure<IEnumerable<YoutubeChannelSearchResult>>(searchResult.Error);
        }

        var channels = searchResult.Value.ToList();

        if (channels.Count == 0)
        {
            return Result.Success<IEnumerable<YoutubeChannelSearchResult>>(channels);
        }

        var subscribedChannels = await youtubeChannelsQuery.GetByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        var subscribedChannelIds = new HashSet<string>(
            subscribedChannels.Select(channel => channel.YoutubeChannelId)
        );

        var results = new List<YoutubeChannelSearchResult>(channels.Count);
        foreach (var channel in channels)
        {
            results.Add(
                channel with
                {
                    AlreadySubscribed = subscribedChannelIds.Contains(channel.ChannelId),
                }
            );
        }

        return Result.Success<IEnumerable<YoutubeChannelSearchResult>>(results);
    }
}
