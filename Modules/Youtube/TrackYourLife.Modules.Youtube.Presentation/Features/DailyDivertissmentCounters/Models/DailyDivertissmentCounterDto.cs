using TrackYourLife.Modules.Youtube.Domain.Features.DailyDivertissmentCounters;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.DailyDivertissmentCounters.Models;

public sealed record DailyDivertissmentCounterDto(DateOnly Date, int VideosWatchedCount);

internal static class DailyDivertissmentCounterDtoExtensions
{
    public static DailyDivertissmentCounterDto ToDto(this DailyDivertissmentCounter counter)
    {
        return new DailyDivertissmentCounterDto(
            Date: counter.Date,
            VideosWatchedCount: counter.VideosWatchedCount
        );
    }

    public static DailyDivertissmentCounterDto ToDto(
        this DailyDivertissmentCounterReadModel counter
    )
    {
        return new DailyDivertissmentCounterDto(
            Date: counter.Date,
            VideosWatchedCount: counter.VideosWatchedCount
        );
    }
}
