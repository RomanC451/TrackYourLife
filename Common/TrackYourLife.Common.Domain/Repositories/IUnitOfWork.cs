namespace TrackYourLife.Common.Domain.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
