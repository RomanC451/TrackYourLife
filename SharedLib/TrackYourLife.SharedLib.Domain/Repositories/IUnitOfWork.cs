using Microsoft.EntityFrameworkCore.Storage;

namespace TrackYourLife.SharedLib.Domain.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    Task ReloadUpdatedEntitiesAsync(CancellationToken cancellationToken);
}
