using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;

public sealed class DailyEntertainmentCounter : Entity<DailyEntertainmentCounterId>
{
    public UserId UserId { get; } = UserId.Empty;
    public DateOnly Date { get; private set; }
    public int VideosWatchedCount { get; private set; }

    private DailyEntertainmentCounter()
        : base() { }

    private DailyEntertainmentCounter(
        DailyEntertainmentCounterId id,
        UserId userId,
        DateOnly date,
        int videosWatchedCount
    )
        : base(id)
    {
        UserId = userId;
        Date = date;
        VideosWatchedCount = videosWatchedCount;
    }

    public static Result<DailyEntertainmentCounter> Create(
        DailyEntertainmentCounterId id,
        UserId userId,
        DateOnly date,
        int videosWatchedCount = 0
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(DailyEntertainmentCounter), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(DailyEntertainmentCounter), nameof(userId))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<DailyEntertainmentCounter>(result.Error);
        }

        if (videosWatchedCount < 0)
        {
            return Result.Failure<DailyEntertainmentCounter>(
                new Error(
                    "Youtube.DailyEntertainmentCounter.InvalidCount",
                    "Videos watched count cannot be negative.",
                    400
                )
            );
        }

        return Result.Success(new DailyEntertainmentCounter(id, userId, date, videosWatchedCount));
    }

    public Result Increment()
    {
        VideosWatchedCount++;
        return Result.Success();
    }

    public bool CanWatchVideo(int maxVideosPerDay)
    {
        return VideosWatchedCount < maxVideosPerDay;
    }
}
