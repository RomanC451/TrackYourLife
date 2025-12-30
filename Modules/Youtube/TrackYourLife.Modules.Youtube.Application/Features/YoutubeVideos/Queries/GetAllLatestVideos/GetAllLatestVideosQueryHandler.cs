using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;

internal sealed class GetAllLatestVideosQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeChannelsQuery youtubeChannelsQuery,
    IYoutubeApiService youtubeApiService
) : IQueryHandler<GetAllLatestVideosQuery, IEnumerable<YoutubeVideoPreview>>
{
    public async Task<Result<IEnumerable<YoutubeVideoPreview>>> Handle(
        GetAllLatestVideosQuery request,
        CancellationToken cancellationToken
    )
    {
        // Get user's saved channels
        IEnumerable<YoutubeChannelReadModel> channels;

        if (request.Category.HasValue)
        {
            channels = await youtubeChannelsQuery.GetByUserIdAndCategoryAsync(
                userIdentifierProvider.UserId,
                request.Category.Value,
                cancellationToken
            );
        }
        else
        {
            channels = await youtubeChannelsQuery.GetByUserIdAsync(
                userIdentifierProvider.UserId,
                cancellationToken
            );
        }

        var channelIds = channels.Select(c => c.YoutubeChannelId).ToList();

        if (channelIds.Count == 0)
        {
            return Result.Success(Enumerable.Empty<YoutubeVideoPreview>());
        }

        // Get videos from all channels
        var videosResult = await youtubeApiService.GetVideosFromChannelsAsync(
            channelIds,
            request.MaxResultsPerChannel,
            cancellationToken
        );

        if (videosResult.IsFailure)
        {
            return videosResult;
        }

        // Sort by publish date descending
        var sortedVideos = videosResult.Value.OrderByDescending(v => v.PublishedAt).ToList();

        return Result.Success<IEnumerable<YoutubeVideoPreview>>(sortedVideos);
    }
}
