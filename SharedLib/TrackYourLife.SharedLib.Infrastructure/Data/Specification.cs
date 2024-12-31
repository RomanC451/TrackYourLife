using System.Linq.Expressions;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.SharedLib.Infrastructure.Data;

public abstract class Specification<TEntity, TId>
    where TEntity : IEntity<TId>
{
    public abstract Expression<Func<TEntity, bool>> ToExpression();

    internal bool IsSatisfiedBy(TEntity entity) => ToExpression().Compile()(entity);

    public static implicit operator Expression<Func<TEntity, bool>>(
        Specification<TEntity, TId> specification
    ) => specification.ToExpression();
}
