using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

public interface IYoutubeCategoriesRepository
{
    Task<YoutubeCategory?> GetByIdAsync(
        YoutubeCategoryId id,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<YoutubeCategory>> ListByUserIdOrderedAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<int> CountByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByUserIdAndNameIgnoreCaseAsync(
        UserId userId,
        string name,
        YoutubeCategoryId? excludeId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(YoutubeCategory category, CancellationToken cancellationToken = default);

    void Update(YoutubeCategory category);

    void Remove(YoutubeCategory category);
}
