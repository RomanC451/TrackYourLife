using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

public interface IYoutubeChannelsQuery
{
    Task<YoutubeChannelReadModel?> GetByIdAsync(
        YoutubeChannelId id,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<YoutubeChannelReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<YoutubeChannelReadModel>> GetByUserIdAndYoutubeCategoryIdAsync(
        UserId userId,
        YoutubeCategoryId categoryId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByUserIdAndYoutubeChannelIdAsync(
        UserId userId,
        string youtubeChannelId,
        CancellationToken cancellationToken = default
    );

    Task<YoutubeChannelReadModel?> GetByUserIdAndYoutubeChannelIdAsync(
        UserId userId,
        string youtubeChannelId,
        CancellationToken cancellationToken = default
    );

    Task<int> CountByUserIdAndYoutubeCategoryIdAsync(
        UserId userId,
        YoutubeCategoryId categoryId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyDictionary<YoutubeCategoryId, int>> CountByUserIdGroupedByCategoryAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );
}
