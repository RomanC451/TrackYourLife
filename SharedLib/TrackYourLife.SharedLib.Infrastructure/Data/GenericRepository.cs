using Microsoft.EntityFrameworkCore;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.SharedLib.Infrastructure.Data;

public abstract class GenericRepository<TEntity, TId>(
    DbSet<TEntity> dbSet,
    IQueryable<TEntity>? query = null
)
    where TEntity : Entity<TId>
    where TId : IStronglyTypedGuid
{
    private readonly DbSet<TEntity> dbSet = dbSet;
    protected readonly IQueryable<TEntity> query = query ?? dbSet;

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken) =>
        await query.FirstOrDefaultAsync(e => Equals(e.Id, id), cancellationToken);

    public async Task<bool> AnyAsync(
        Specification<TEntity, TId> specification,
        CancellationToken cancellationToken
    ) => await query.AnyAsync(specification, cancellationToken);

    protected async Task<TEntity?> FirstOrDefaultAsync(
        Specification<TEntity, TId> specification,
        CancellationToken cancellationToken
    ) => await query.FirstOrDefaultAsync(specification, cancellationToken);

    protected async Task<List<TEntity>> WhereAsync(
        Specification<TEntity, TId> specification,
        CancellationToken cancellationToken
    ) => await query.Where(specification).ToListAsync(cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
        await dbSet.AddAsync(entity, cancellationToken);

    public void AddRange(IEnumerable<TEntity> entities) => dbSet.AddRange(entities);

    public async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken
    ) => await dbSet.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity) => dbSet.Update(entity);

    public void Remove(TEntity entity) => dbSet.Remove(entity);
}
