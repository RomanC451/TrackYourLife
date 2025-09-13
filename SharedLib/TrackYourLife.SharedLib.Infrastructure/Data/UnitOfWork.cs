using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using TrackYourLife.SharedLib.Domain.OutboxMessages;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Repositories;

namespace TrackYourLife.SharedLib.Infrastructure.Data;

public class UnitOfWork<DbType>(DbType dbContext) : IUnitOfWork
    where DbType : DbContext
{
    private readonly DbType _dbContext = dbContext;

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        UpdateAuditableEntities();

        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public IReadOnlyCollection<IDirectDomainEvent> GetDirectDomainEvents()
    {
        // RaiseOnDeleteDomainEvents();

        var domainEvents = _dbContext
            .ChangeTracker.Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDirectDomainEvents();
                aggregateRoot.ClearDirectDomainEvents();
                return domainEvents;
            })
            .ToList();

        return domainEvents;
    }

    public async Task AddOutboxMessageAsync(
        OutboxMessage outboxMessage,
        CancellationToken cancellationToken
    )
    {
        await _dbContext.Set<OutboxMessage>().AddAsync(outboxMessage, cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<IAuditableEntity>> entries =
            _dbContext.ChangeTracker.Entries<IAuditableEntity>();

        foreach (EntityEntry<IAuditableEntity> entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.CreatedOnUtc).CurrentValue = DateTime.UtcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.ModifiedOnUtc).CurrentValue = DateTime.UtcNow;
            }
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken
    )
    {
        return await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task ReloadUpdatedEntitiesAsync(CancellationToken cancellationToken)
    {
        var entries = _dbContext.ChangeTracker.Entries().ToList();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Modified
                or EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    await entry.ReloadAsync(cancellationToken);
                    if (entry.Entity is IAggregateRoot aggregateRoot)
                    {
                        aggregateRoot.ClearDirectDomainEvents();
                        aggregateRoot.ClearOutboxDomainEvents();
                    }
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
            }
        }
    }
}
