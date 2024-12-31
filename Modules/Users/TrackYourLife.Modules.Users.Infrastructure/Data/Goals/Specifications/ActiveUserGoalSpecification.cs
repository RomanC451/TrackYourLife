using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Goals.Specifications;

internal class ActiveUserGoalSpecification(UserId userId, GoalType type)
    : Specification<Goal, GoalId>
{
    public override Expression<Func<Goal, bool>> ToExpression() =>
        goal => goal.UserId == userId && goal.Type == type && goal.EndDate == DateOnly.MaxValue;

    internal Expression<Func<GoalReadModel, bool>> ToReadModelExpression() =>
        goal => goal.UserId == userId && goal.Type == type && goal.EndDate == DateOnly.MaxValue;
}
