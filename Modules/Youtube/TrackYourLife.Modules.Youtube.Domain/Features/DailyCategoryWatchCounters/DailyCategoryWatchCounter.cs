using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;

public sealed class DailyCategoryWatchCounter : Entity<DailyCategoryWatchCounterId>
{
    public UserId UserId { get; } = UserId.Empty;
    public DateOnly Date { get; private set; }
    public YoutubeCategoryId YoutubeCategoryId { get; private set; } = YoutubeCategoryId.Empty;
    public int VideosWatchedCount { get; private set; }

    private DailyCategoryWatchCounter()
        : base() { }

    private DailyCategoryWatchCounter(
        DailyCategoryWatchCounterId id,
        UserId userId,
        DateOnly date,
        YoutubeCategoryId youtubeCategoryId,
        int videosWatchedCount
    )
        : base(id)
    {
        UserId = userId;
        Date = date;
        YoutubeCategoryId = youtubeCategoryId;
        VideosWatchedCount = videosWatchedCount;
    }

    public static Result<DailyCategoryWatchCounter> Create(
        DailyCategoryWatchCounterId id,
        UserId userId,
        DateOnly date,
        YoutubeCategoryId youtubeCategoryId,
        int videosWatchedCount = 0
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(DailyCategoryWatchCounter), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(DailyCategoryWatchCounter), nameof(userId))
            ),
            Ensure.NotEmptyId(
                youtubeCategoryId,
                DomainErrors.ArgumentError.Empty(
                    nameof(DailyCategoryWatchCounter),
                    nameof(youtubeCategoryId)
                )
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<DailyCategoryWatchCounter>(result.Error);
        }

        if (videosWatchedCount < 0)
        {
            return Result.Failure<DailyCategoryWatchCounter>(
                new Error(
                    "Youtube.DailyCategoryWatchCounter.InvalidCount",
                    "Videos watched count cannot be negative.",
                    400
                )
            );
        }

        return Result.Success(
            new DailyCategoryWatchCounter(id, userId, date, youtubeCategoryId, videosWatchedCount)
        );
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
