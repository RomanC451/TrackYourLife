using TrackYourLifeDotnet.Domain.Primitives;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace TrackYourLifeDotnet.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // ConvertDomainEventsToOutboxMessages();
        // UpdateAuditableEntities();

        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    // private void ConvertDomainEventsToOutboxMessages()
    // {
    //     var outboxMessages = _dbContext.ChangeTracker
    //         .Entries<AggregateRoot>()
    //         .Select(x => x.Entity)
    //         .SelectMany(aggregateRoot =>
    //         {
    //             var domainEvents = aggregateRoot.GetDomainEvents();

    //             aggregateRoot.ClearDomainEvents();

    //             return domainEvents;
    //         })
    //         .Select(
    //             domainEvent =>
    //                 new OutboxMessage
    //                 {
    //                     Id = Guid.NewGuid(),
    //                     OccurredOnUtc = DateTime.UtcNow,
    //                     Type = domainEvent.GetType().Name,
    //                     Content = JsonConvert.SerializeObject(
    //                         domainEvent,
    //                         new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
    //                     )
    //                 }
    //         )
    //         .ToList();

    //     _dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
    // }

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
}
