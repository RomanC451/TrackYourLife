using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

public interface IYoutubeCategoriesQuery
{
    Task<IReadOnlyList<YoutubeCategoryReadModel>> ListByUserIdOrderedAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<YoutubeCategoryReadModel?> GetByIdAsync(
        YoutubeCategoryId id,
        CancellationToken cancellationToken = default
    );
}
