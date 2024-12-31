using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
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
}
