using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyDivertissmentCounters;

public sealed class DailyDivertissmentCounter : Entity<DailyDivertissmentCounterId>
{
    public UserId UserId { get; } = UserId.Empty;
    public DateOnly Date { get; private set; }
    public int VideosWatchedCount { get; private set; }

    private DailyDivertissmentCounter()
        : base() { }

    private DailyDivertissmentCounter(
        DailyDivertissmentCounterId id,
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

    public static Result<DailyDivertissmentCounter> Create(
        DailyDivertissmentCounterId id,
        UserId userId,
        DateOnly date,
        int videosWatchedCount = 0
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(DailyDivertissmentCounter), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(DailyDivertissmentCounter), nameof(userId))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<DailyDivertissmentCounter>(result.Error);
        }

        if (videosWatchedCount < 0)
        {
            return Result.Failure<DailyDivertissmentCounter>(
                new Error(
                    "Youtube.DailyDivertissmentCounter.InvalidCount",
                    "Videos watched count cannot be negative.",
                    400
                )
            );
        }

        return Result.Success(new DailyDivertissmentCounter(id, userId, date, videosWatchedCount));
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
