using TrackYourLife.Modules.Trainings.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.Outbox;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Outbox;

internal sealed class TrainingsOutboxMessageRepository(TrainingsWriteDbContext dbContext)
    : OutboxMessageRepository(dbContext.OutboxMessages),
        ITrainingsOutboxMessageRepository;
