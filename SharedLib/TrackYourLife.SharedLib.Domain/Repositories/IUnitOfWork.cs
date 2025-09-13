using Microsoft.EntityFrameworkCore.Storage;
using TrackYourLife.SharedLib.Domain.OutboxMessages;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.SharedLib.Domain.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    Task ReloadUpdatedEntitiesAsync(CancellationToken cancellationToken);

    IReadOnlyCollection<IDirectDomainEvent> GetDirectDomainEvents();

    Task AddOutboxMessageAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken);
}
