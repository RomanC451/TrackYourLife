using TrackYourLife.Modules.Youtube.Domain.Core;
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

    Task<IEnumerable<YoutubeChannelReadModel>> GetByUserIdAndCategoryAsync(
        UserId userId,
        VideoCategory category,
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
}

