using TrackYourLife.Modules.GuitarTabs.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.Outbox;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.Outbox;

internal sealed class GuitarTabsOutboxMessageRepository(GuitarTabsWriteDbContext dbContext)
    : OutboxMessageRepository(dbContext.OutboxMessages),
        IGuitarTabsOutboxMessageRepository;
