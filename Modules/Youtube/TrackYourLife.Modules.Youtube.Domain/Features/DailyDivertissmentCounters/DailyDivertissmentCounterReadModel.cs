using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyDivertissmentCounters;

public sealed record DailyDivertissmentCounterReadModel(
    DailyDivertissmentCounterId Id,
    UserId UserId,
    DateOnly Date,
    int VideosWatchedCount
) : IReadModel<DailyDivertissmentCounterId>;
