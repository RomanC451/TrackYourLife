using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.DailyCategoryWatchCounters.Models;

public sealed record DailyCategoryWatchCounterDto(
    Guid Id,
    Guid YoutubeCategoryId,
    DateOnly Date,
    int VideosWatchedCount
);

internal static class DailyCategoryWatchCounterDtoExtensions
{
    public static DailyCategoryWatchCounterDto ToDto(this DailyCategoryWatchCounterReadModel model)
    {
        return new DailyCategoryWatchCounterDto(
            model.Id.Value,
            model.YoutubeCategoryId.Value,
            model.Date,
            model.VideosWatchedCount
        );
    }
}
