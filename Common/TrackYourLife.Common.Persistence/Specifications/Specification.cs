using System.Linq.Expressions;
using TrackYourLife.Common.Domain.Primitives;

namespace TrackYourLife.Common.Persistence.Specifications;

public abstract class Specification<TEntity, TId>
    where TEntity : Entity<TId>
{
    public abstract Expression<Func<TEntity, bool>> ToExpression();

    internal bool IsSatisfiedBy(TEntity entity) => ToExpression().Compile()(entity);

    public static implicit operator Expression<Func<TEntity, bool>>(
        Specification<TEntity, TId> specification
    ) => specification.ToExpression();
}
