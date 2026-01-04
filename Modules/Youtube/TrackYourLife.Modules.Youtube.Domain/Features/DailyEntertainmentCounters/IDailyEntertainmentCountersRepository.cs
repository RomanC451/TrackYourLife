using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;

public interface IDailyEntertainmentCountersRepository
{
    Task<DailyEntertainmentCounter?> GetByUserIdAndDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(DailyEntertainmentCounter counter, CancellationToken cancellationToken = default);

    void Update(DailyEntertainmentCounter counter);
}
