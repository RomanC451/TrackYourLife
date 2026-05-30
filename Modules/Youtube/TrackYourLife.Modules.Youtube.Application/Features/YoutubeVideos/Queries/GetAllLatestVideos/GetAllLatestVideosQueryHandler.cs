using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;

internal sealed class GetAllLatestVideosQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeChannelsQuery youtubeChannelsQuery,
    IYoutubeApiService youtubeApiService,
    IWatchedVideosRepository watchedVideosRepository
) : IQueryHandler<GetAllLatestVideosQuery, IEnumerable<YoutubeVideoPreview>>
{
    public async Task<Result<IEnumerable<YoutubeVideoPreview>>> Handle(
        GetAllLatestVideosQuery request,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<YoutubeChannelReadModel> channels;

        if (request.FavoritesOnly)
        {
            channels = await youtubeChannelsQuery.GetFavoritesByUserIdAsync(
                userIdentifierProvider.UserId,
                cancellationToken
            );
        }
        else if (request.YoutubeCategoryId is { } categoryId)
        {
            channels = await youtubeChannelsQuery.GetByUserIdAndYoutubeCategoryIdAsync(
                userIdentifierProvider.UserId,
                categoryId,
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

        var videosResult = await youtubeApiService.GetVideosFromChannelsAsync(
            channelIds,
            request.MaxResultsPerChannel,
            cancellationToken
        );

        if (videosResult.IsFailure)
        {
            return videosResult;
        }

        var sortedVideos = videosResult.Value.OrderByDescending(v => v.PublishedAt).ToList();

        var videoIds = sortedVideos.Select(v => v.VideoId).ToList();
        var watchedVideos = await watchedVideosRepository.GetByUserIdAndVideoIdsAsync(
            userIdentifierProvider.UserId,
            videoIds,
            cancellationToken
        );

        var watchedVideoIds = new HashSet<string>(watchedVideos.Select(w => w.VideoId));

        var videosWithWatchedStatus = sortedVideos
            .Select(video => video with { IsWatched = watchedVideoIds.Contains(video.VideoId) })
            .ToList();

        return Result.Success<IEnumerable<YoutubeVideoPreview>>(videosWithWatchedStatus);
    }
}
