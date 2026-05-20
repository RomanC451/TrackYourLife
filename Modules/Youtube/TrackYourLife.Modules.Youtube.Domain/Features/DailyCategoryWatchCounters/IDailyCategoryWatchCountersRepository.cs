using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;

public interface IDailyCategoryWatchCountersRepository
{
    Task<DailyCategoryWatchCounter?> GetByUserIdDateAndCategoryAsync(
        UserId userId,
        DateOnly date,
        YoutubeCategoryId categoryId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(DailyCategoryWatchCounter counter, CancellationToken cancellationToken = default);

    void Update(DailyCategoryWatchCounter counter);
}
