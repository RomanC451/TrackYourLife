using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;

public interface IDailyCategoryWatchCountersQuery
{
    Task<IReadOnlyList<DailyCategoryWatchCounterReadModel>> ListByUserIdAndDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default
    );
}
