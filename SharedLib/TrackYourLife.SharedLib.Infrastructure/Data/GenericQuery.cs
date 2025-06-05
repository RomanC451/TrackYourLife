using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.SharedLib.Infrastructure.Data;

public abstract class GenericQuery<TReadModel, TId>(IQueryable<TReadModel> query)
    where TReadModel : class, IReadModel<TId>
{
    protected readonly IQueryable<TReadModel> query = query;

    public async Task<TReadModel?> GetByIdAsync(TId id, CancellationToken cancellationToken) =>
        await query.FirstOrDefaultAsync(e => Equals(e.Id, id), cancellationToken);

    public async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken) =>
        await query.AnyAsync(e => Equals(e.Id, id), cancellationToken);

    protected Task<TReadModel?> FirstOrDefaultAsync(
        Specification<TReadModel, TId> specification,
        CancellationToken cancellationToken
    ) => query.FirstOrDefaultAsync(specification, cancellationToken);

    protected async Task<IEnumerable<TReadModel>> WhereAsync(
        Specification<TReadModel, TId> specification,
        CancellationToken cancellationToken
    ) => await Task.Run(() => query.Where(specification), cancellationToken);

    protected async Task<IEnumerable<TReadModel>> WhereAsync(
        Expression<Func<TReadModel, bool>> expression,
        CancellationToken cancellationToken
    ) => await Task.Run(() => query.Where(expression), cancellationToken);

    protected async Task<bool> AnyAsync(
        Specification<TReadModel, TId> specification,
        CancellationToken cancellationToken
    ) => await query.AnyAsync(specification, cancellationToken);

    protected async Task<bool> AnyAsync(
        Expression<Func<TReadModel, bool>> expression,
        CancellationToken cancellationToken
    ) => await query.AnyAsync(expression, cancellationToken);
}
