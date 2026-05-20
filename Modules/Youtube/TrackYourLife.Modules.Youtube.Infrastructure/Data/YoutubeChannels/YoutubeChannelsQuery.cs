using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
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

    public async Task<IEnumerable<YoutubeChannelReadModel>> GetByUserIdAndYoutubeCategoryIdAsync(
        UserId userId,
        YoutubeCategoryId categoryId,
        CancellationToken cancellationToken = default
    )
    {
        return await WhereAsync(
            new YoutubeChannelReadModelWithUserIdAndYoutubeCategorySpecification(userId, categoryId),
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

    public async Task<int> CountByUserIdAndYoutubeCategoryIdAsync(
        UserId userId,
        YoutubeCategoryId categoryId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.YoutubeChannels.CountAsync(
            c => c.UserId == userId && c.YoutubeCategoryId == categoryId,
            cancellationToken
        );
    }

    public async Task<IReadOnlyDictionary<YoutubeCategoryId, int>> CountByUserIdGroupedByCategoryAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        var rows = await dbContext
            .YoutubeChannels.AsNoTracking()
            .Where(c => c.UserId == userId)
            .GroupBy(c => c.YoutubeCategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(r => r.CategoryId, r => r.Count);
    }
}
