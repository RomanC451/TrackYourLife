using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetVideoDetails;

internal sealed class GetVideoDetailsQueryHandler(
    IYoutubeApiService youtubeApiService,
    IUserIdentifierProvider userIdentifierProvider,
    IWatchedVideosRepository watchedVideosRepository
) : IQueryHandler<GetVideoDetailsQuery, YoutubeVideoDetails>
{
    public async Task<Result<YoutubeVideoDetails>> Handle(
        GetVideoDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        // Check if video is in watched videos database
        var existingWatchedVideo = await watchedVideosRepository.GetByUserIdAndVideoIdAsync(
            userId,
            request.VideoId,
            cancellationToken
        );

        // If video is not watched, return error
        if (existingWatchedVideo is null)
        {
            return Result.Failure<YoutubeVideoDetails>(
                WatchedVideoErrors.NotFound(request.VideoId)
            );
        }

        // If video is watched, return details from API (cached)
        return await youtubeApiService.GetVideoDetailsAsync(request.VideoId, cancellationToken);
    }
}
