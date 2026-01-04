using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.DailyEntertainmentCounters.Models;

public sealed record DailyEntertainmentCounterDto(DateOnly Date, int VideosWatchedCount);

internal static class DailyEntertainmentCounterDtoExtensions
{
    public static DailyEntertainmentCounterDto ToDto(this DailyEntertainmentCounter counter)
    {
        return new DailyEntertainmentCounterDto(
            Date: counter.Date,
            VideosWatchedCount: counter.VideosWatchedCount
        );
    }

    public static DailyEntertainmentCounterDto ToDto(
        this DailyEntertainmentCounterReadModel counter
    )
    {
        return new DailyEntertainmentCounterDto(
            Date: counter.Date,
            VideosWatchedCount: counter.VideosWatchedCount
        );
    }
}
