using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetHomeRecommendation;

internal sealed class GetHomeRecommendationQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeChannelsQuery youtubeChannelsQuery,
    IYoutubeApiService youtubeApiService,
    IWatchedVideosRepository watchedVideosRepository
) : IQueryHandler<GetHomeRecommendationQuery, YoutubeVideoPreview?>
{
    private const int MaxResultsPerChannel = 2;

    public async Task<Result<YoutubeVideoPreview?>> Handle(
        GetHomeRecommendationQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var favoriteChannels = await youtubeChannelsQuery.GetFavoritesByUserIdAsync(
            userId,
            cancellationToken
        );

        if (favoriteChannels.Count == 0)
        {
            return Result.Success<YoutubeVideoPreview?>(null);
        }

        var channelIds = favoriteChannels.Select(channel => channel.YoutubeChannelId).ToList();

        var videosResult = await youtubeApiService.GetVideosFromChannelsAsync(
            channelIds,
            MaxResultsPerChannel,
            cancellationToken
        );

        if (videosResult.IsFailure)
        {
            return Result.Failure<YoutubeVideoPreview?>(videosResult.Error);
        }

        var videos = videosResult.Value.ToList();

        if (videos.Count == 0)
        {
            return Result.Success<YoutubeVideoPreview?>(null);
        }

        var videoIds = videos.Select(video => video.VideoId).ToList();
        var watchedVideos = await watchedVideosRepository.GetByUserIdAndVideoIdsAsync(
            userId,
            videoIds,
            cancellationToken
        );

        var watchedVideoIds = new HashSet<string>(watchedVideos.Select(watched => watched.VideoId));

        var videosWithWatchedStatus = videos
            .Select(video => video with { IsWatched = watchedVideoIds.Contains(video.VideoId) })
            .ToList();

        var videosByChannel = favoriteChannels
            .Select(channel =>
                (IReadOnlyList<YoutubeVideoPreview>)
                    videosWithWatchedStatus
                        .Where(video => video.ChannelId == channel.YoutubeChannelId)
                        .ToList()
            )
            .ToList();

        var recommendation = HomeRecommendationSelector.Pick(videosByChannel);

        return Result.Success(recommendation);
    }
}
