using TrackYourLife.Modules.Reading.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.Outbox;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.Outbox;

internal sealed class ReadingOutboxMessageRepository(ReadingWriteDbContext dbContext)
    : OutboxMessageRepository(dbContext.OutboxMessages),
        IReadingOutboxMessageRepository;
