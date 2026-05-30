using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetLatestVideosFromChannel;

internal sealed class GetLatestVideosFromChannelQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeApiService youtubeApiService,
    IWatchedVideosRepository watchedVideosRepository
) : IQueryHandler<GetLatestVideosFromChannelQuery, IEnumerable<YoutubeVideoPreview>>
{
    public async Task<Result<IEnumerable<YoutubeVideoPreview>>> Handle(
        GetLatestVideosFromChannelQuery request,
        CancellationToken cancellationToken
    )
    {
        var videosResult = await youtubeApiService.GetChannelVideosAsync(
            request.ChannelId,
            request.MaxResults,
            cancellationToken
        );

        if (videosResult.IsFailure)
        {
            return videosResult;
        }

        var videos = videosResult.Value.ToList();
        var videoIds = videos.Select(v => v.VideoId).ToList();
        var watchedVideos = await watchedVideosRepository.GetByUserIdAndVideoIdsAsync(
            userIdentifierProvider.UserId,
            videoIds,
            cancellationToken
        );

        var watchedVideoIds = new HashSet<string>(watchedVideos.Select(w => w.VideoId));

        var videosWithWatchedStatus = videos
            .Select(video => video with { IsWatched = watchedVideoIds.Contains(video.VideoId) })
            .ToList();

        return Result.Success<IEnumerable<YoutubeVideoPreview>>(videosWithWatchedStatus);
    }
}

