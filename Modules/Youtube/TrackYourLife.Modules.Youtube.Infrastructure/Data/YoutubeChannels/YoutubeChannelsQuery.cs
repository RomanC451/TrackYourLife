using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeChannels.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeChannels;

internal sealed class YoutubeChannelsQuery(YoutubeReadDbContext dbContext)
    : GenericQuery<YoutubeChannelReadModel, YoutubeChannelId>(dbContext.YoutubeChannels),
        IYoutubeChannelsQuery
{
    public async Task<IEnumerable<YoutubeChannelReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await WhereAsync(
            new YoutubeChannelReadModelWithUserIdSpecification(userId),
            cancellationToken
        );
    }

    public async Task<IEnumerable<YoutubeChannelReadModel>> GetByUserIdAndCategoryAsync(
        UserId userId,
        VideoCategory category,
        CancellationToken cancellationToken = default
    )
    {
        return await WhereAsync(
            new YoutubeChannelReadModelWithUserIdAndCategorySpecification(userId, category),
            cancellationToken
        );
    }

    public async Task<bool> ExistsByUserIdAndYoutubeChannelIdAsync(
        UserId userId,
        string youtubeChannelId,
        CancellationToken cancellationToken = default
    )
    {
        return await AnyAsync(
            new YoutubeChannelReadModelWithUserIdAndYoutubeChannelIdSpecification(
                userId,
                youtubeChannelId
            ),
            cancellationToken
        );
    }

    public async Task<YoutubeChannelReadModel?> GetByUserIdAndYoutubeChannelIdAsync(
        UserId userId,
        string youtubeChannelId,
        CancellationToken cancellationToken = default
    )
    {
        return await FirstOrDefaultAsync(
            new YoutubeChannelReadModelWithUserIdAndYoutubeChannelIdSpecification(
                userId,
                youtubeChannelId
            ),
            cancellationToken
        );
    }
}

