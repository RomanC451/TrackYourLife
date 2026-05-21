using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeChannels;

internal sealed class YoutubeChannelsRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<YoutubeChannel, YoutubeChannelId>(dbContext.YoutubeChannels),
        IYoutubeChannelsRepository
{
    public async Task<YoutubeChannel?> GetByUserIdAndYoutubeChannelIdAsync(
        UserId userId,
        string youtubeChannelId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.YoutubeChannels.FirstOrDefaultAsync(
            c => c.UserId == userId && c.YoutubeChannelId == youtubeChannelId,
            cancellationToken
        );
    }

    public async Task<int> RemoveAllByUserIdAndCategoryIdAsync(
        UserId userId,
        YoutubeCategoryId categoryId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .YoutubeChannels.Where(c => c.UserId == userId && c.YoutubeCategoryId == categoryId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<YoutubeChannel>> ListByUserIdAndCategoryIdAsync(
        UserId userId,
        YoutubeCategoryId categoryId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .YoutubeChannels.Where(c => c.UserId == userId && c.YoutubeCategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }
}
