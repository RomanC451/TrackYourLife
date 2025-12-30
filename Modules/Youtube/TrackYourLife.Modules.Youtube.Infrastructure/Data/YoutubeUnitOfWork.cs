using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data;

internal sealed class YoutubeUnitOfWork(YoutubeWriteDbContext dbContext)
    : UnitOfWork<YoutubeWriteDbContext>(dbContext),
        IYoutubeUnitOfWork;

