using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;

public sealed record DailyEntertainmentCounterReadModel(
    DailyEntertainmentCounterId Id,
    UserId UserId,
    DateOnly Date,
    int VideosWatchedCount
) : IReadModel<DailyEntertainmentCounterId>;
