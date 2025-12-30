using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeChannels;

internal sealed class YoutubeChannelsRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<YoutubeChannel, YoutubeChannelId>(dbContext.YoutubeChannels),
        IYoutubeChannelsRepository;

