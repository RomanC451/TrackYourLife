using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;

public sealed record DailyCategoryWatchCounterReadModel(
    DailyCategoryWatchCounterId Id,
    UserId UserId,
    DateOnly Date,
    YoutubeCategoryId YoutubeCategoryId,
    int VideosWatchedCount
) : IReadModel<DailyCategoryWatchCounterId>;
