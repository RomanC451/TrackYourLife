using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Commands.PlayVideo;

internal sealed class PlayVideoCommandHandler(
    IYoutubeApiService youtubeApiService,
    IUserIdentifierProvider userIdentifierProvider,
    IWatchedVideosRepository watchedVideosRepository,
    IYoutubeSettingsRepository youtubeSettingsRepository,
    IDailyEntertainmentCountersRepository dailyEntertainmentCountersRepository,
    IYoutubeChannelsQuery youtubeChannelsQuery,
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

        // 1. Check if video already watched
        var existingWatchedVideo = await watchedVideosRepository.GetByUserIdAndVideoIdAsync(
            userId,
            request.VideoId,
            cancellationToken
        );

        if (existingWatchedVideo is not null)
        {
            // Video already watched, return details from API (cached)
            return await youtubeApiService.GetVideoDetailsAsync(request.VideoId, cancellationToken);
        }

        // 2. Get video details from API to get channelId
        var videoDetailsResult = await youtubeApiService.GetVideoDetailsAsync(
            request.VideoId,
            cancellationToken
        );

        if (videoDetailsResult.IsFailure)
        {
            return videoDetailsResult;
        }

        var videoDetails = videoDetailsResult.Value;

        // 3. Get user's settings (or use defaults)
        var settings = await youtubeSettingsRepository.GetByUserIdAsync(userId, cancellationToken);
        var maxEntertainmentVideosPerDay = settings?.MaxEntertainmentVideosPerDay ?? 0;

        // 4. Get today's counter (or create if doesn't exist)
        var counter = await dailyEntertainmentCountersRepository.GetByUserIdAndDateAsync(
            userId,
            today,
            cancellationToken
        );

        if (counter is null)
        {
            var counterId = DailyEntertainmentCounterId.NewId();
            var createCounterResult = DailyEntertainmentCounter.Create(counterId, userId, today, 0);

            if (createCounterResult.IsFailure)
            {
                return Result.Failure<YoutubeVideoDetails>(createCounterResult.Error);
            }

            counter = createCounterResult.Value;
            await dailyEntertainmentCountersRepository.AddAsync(counter, cancellationToken);
        }

        // 5. Check if channel is in database and determine if it's divertissment
        var channel = await youtubeChannelsQuery.GetByUserIdAndYoutubeChannelIdAsync(
            userId,
            videoDetails.ChannelId,
            cancellationToken
        );

        var isEntertainment = channel is null || channel.Category == VideoCategory.Entertainment;

        // 6. Check limit if it's a divertissment video
        if (
            maxEntertainmentVideosPerDay > 0
            && isEntertainment
            && !counter.CanWatchVideo(maxEntertainmentVideosPerDay)
        )
        {
            return Result.Failure<YoutubeVideoDetails>(
                YoutubeSettingsErrors.EntertainmentLimitReached
            );
        }

        // 7. Create watched video entry
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

        // 8. Increment counter if it's a divertissment video
        if (isEntertainment)
        {
            var incrementResult = counter.Increment();
            if (incrementResult.IsFailure)
            {
                return Result.Failure<YoutubeVideoDetails>(incrementResult.Error);
            }
        }

        // 9. Return video details
        return Result.Success(videoDetails);
    }
}
