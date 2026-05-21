using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Commands.PlayVideo;

internal sealed class PlayVideoCommandHandler(
    IYoutubeApiService youtubeApiService,
    IUserIdentifierProvider userIdentifierProvider,
    IWatchedVideosRepository watchedVideosRepository,
    IDailyCategoryWatchCountersRepository dailyCategoryWatchCountersRepository,
    IYoutubeChannelsQuery youtubeChannelsQuery,
    IYoutubeCategoriesQuery youtubeCategoriesQuery,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<PlayVideoCommand, YoutubeVideoDetails>
{
    public async Task<Result<YoutubeVideoDetails>> Handle(
        PlayVideoCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var utcNow = dateTimeProvider.UtcNow;
        var today = DateOnly.FromDateTime(utcNow);

        var existingWatchedVideo = await watchedVideosRepository.GetByUserIdAndVideoIdAsync(
            userId,
            request.VideoId,
            cancellationToken
        );

        if (existingWatchedVideo is not null)
        {
            return await youtubeApiService.GetVideoDetailsAsync(request.VideoId, cancellationToken);
        }

        var videoDetailsResult = await youtubeApiService.GetVideoDetailsAsync(
            request.VideoId,
            cancellationToken
        );

        if (videoDetailsResult.IsFailure)
        {
            return videoDetailsResult;
        }

        var videoDetails = videoDetailsResult.Value;

        var channel = await youtubeChannelsQuery.GetByUserIdAndYoutubeChannelIdAsync(
            userId,
            videoDetails.ChannelId,
            cancellationToken
        );

        if (channel is null)
        {
            var watchedVideoId = WatchedVideoId.NewId();
            var createWatchedVideoResult = WatchedVideo.Create(
                watchedVideoId,
                userId,
                videoDetails.VideoId,
                videoDetails.ChannelId,
                utcNow
            );

            if (createWatchedVideoResult.IsFailure)
            {
                return Result.Failure<YoutubeVideoDetails>(createWatchedVideoResult.Error);
            }

            await watchedVideosRepository.AddAsync(createWatchedVideoResult.Value, cancellationToken);
            return Result.Success(videoDetails);
        }

        var category = await youtubeCategoriesQuery.GetByIdAsync(
            channel.YoutubeCategoryId,
            cancellationToken
        );

        if (category is null)
        {
            return Result.Failure<YoutubeVideoDetails>(YoutubeCategoriesErrors.NotFound);
        }

        var maxVideosPerDay = category.MaxVideosPerDay;

        if (maxVideosPerDay > 0)
        {
            var counter = await dailyCategoryWatchCountersRepository.GetByUserIdDateAndCategoryAsync(
                userId,
                today,
                channel.YoutubeCategoryId,
                cancellationToken
            );

            var isNewCounter = counter is null;
            DailyCategoryWatchCounter activeCounter;
            if (isNewCounter)
            {
                var counterId = DailyCategoryWatchCounterId.NewId();
                var createCounterResult = DailyCategoryWatchCounter.Create(
                    counterId,
                    userId,
                    today,
                    channel.YoutubeCategoryId,
                    0
                );

                if (createCounterResult.IsFailure)
                {
                    return Result.Failure<YoutubeVideoDetails>(createCounterResult.Error);
                }

                activeCounter = createCounterResult.Value;
                await dailyCategoryWatchCountersRepository.AddAsync(activeCounter, cancellationToken);
            }
            else
            {
                activeCounter = counter!;
            }

            if (!activeCounter.CanWatchVideo(maxVideosPerDay))
            {
                return Result.Failure<YoutubeVideoDetails>(YoutubeCategoriesErrors.CategoryLimitReached);
            }

            var watchedVideoId2 = WatchedVideoId.NewId();
            var createWatchedVideoResult2 = WatchedVideo.Create(
                watchedVideoId2,
                userId,
                videoDetails.VideoId,
                videoDetails.ChannelId,
                utcNow
            );

            if (createWatchedVideoResult2.IsFailure)
            {
                return Result.Failure<YoutubeVideoDetails>(createWatchedVideoResult2.Error);
            }

            await watchedVideosRepository.AddAsync(createWatchedVideoResult2.Value, cancellationToken);

            var incrementResult = activeCounter.Increment();
            if (incrementResult.IsFailure)
            {
                return Result.Failure<YoutubeVideoDetails>(incrementResult.Error);
            }

            // New counters are already tracked as Added; Update() would force Modified and UPDATE 0 rows.
            if (!isNewCounter)
            {
                dailyCategoryWatchCountersRepository.Update(activeCounter);
            }

            return Result.Success(videoDetails);
        }

        var watchedVideoId3 = WatchedVideoId.NewId();
        var createWatchedVideoResult3 = WatchedVideo.Create(
            watchedVideoId3,
            userId,
            videoDetails.VideoId,
            videoDetails.ChannelId,
            utcNow
        );

        if (createWatchedVideoResult3.IsFailure)
        {
            return Result.Failure<YoutubeVideoDetails>(createWatchedVideoResult3.Error);
        }

        await watchedVideosRepository.AddAsync(createWatchedVideoResult3.Value, cancellationToken);
        return Result.Success(videoDetails);
    }
}
