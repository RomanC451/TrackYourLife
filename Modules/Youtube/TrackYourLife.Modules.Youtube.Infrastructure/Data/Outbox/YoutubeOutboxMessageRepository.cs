using TrackYourLife.Modules.Youtube.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.Outbox;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.Outbox;

internal sealed class YoutubeOutboxMessageRepository(YoutubeWriteDbContext dbContext)
    : OutboxMessageRepository(dbContext.OutboxMessages),
        IYoutubeOutboxMessageRepository;

