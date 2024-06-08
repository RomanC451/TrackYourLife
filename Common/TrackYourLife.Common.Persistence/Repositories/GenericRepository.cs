using Microsoft.EntityFrameworkCore;
using TrackYourLife.Common.Persistence.Specifications;
using TrackYourLife.Common.Domain.Primitives;

namespace TrackYourLife.Common.Persistence.Repositories;

public abstract class GenericRepository<TEntity, TId>
    where TEntity : Entity<TId>
{
    protected DbSet<TEntity> _dbSet { get; }

    protected GenericRepository(DbSet<TEntity> dbSet) => _dbSet = dbSet;

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken) =>
        await _dbSet.FirstAsync(e => Equals(e.Id, id), cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
        await _dbSet.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(
        IReadOnlyCollection<TEntity> entities,
        CancellationToken cancellationToken
    ) => await _dbSet.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public void Remove(TEntity entity) => _dbSet.Remove(entity);

    public async Task<bool> AnyAsync(
        Specification<TEntity, TId> specification,
        CancellationToken cancellationToken
    ) => await _dbSet.AnyAsync(specification, cancellationToken);

    protected async Task<TEntity?> FirstOrDefaultAsync(
        Specification<TEntity, TId> specification,
        CancellationToken cancellationToken
    ) => await _dbSet.FirstOrDefaultAsync(specification, cancellationToken);

    protected async Task<List<TEntity>> WhereAsync(
        Specification<TEntity, TId> specification,
        CancellationToken cancellationToken
    ) => await _dbSet.Where(specification).ToListAsync(cancellationToken);
}
