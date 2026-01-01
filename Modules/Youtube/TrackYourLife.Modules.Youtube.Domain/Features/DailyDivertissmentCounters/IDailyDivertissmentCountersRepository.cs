using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyDivertissmentCounters;

public interface IDailyDivertissmentCountersRepository
{
    Task<DailyDivertissmentCounter?> GetByUserIdAndDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(DailyDivertissmentCounter counter, CancellationToken cancellationToken = default);

    void Update(DailyDivertissmentCounter counter);
}
