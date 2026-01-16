using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeChannels;

internal sealed class YoutubeChannelsRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<YoutubeChannel, YoutubeChannelId>(dbContext.YoutubeChannels),
        IYoutubeChannelsRepository
{
    public async Task<YoutubeChannel?> GetByYoutubeChannelIdAsync(
        string youtubeChannelId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.YoutubeChannels.FirstOrDefaultAsync(
            c => c.YoutubeChannelId == youtubeChannelId,
            cancellationToken
        );
    }
}
