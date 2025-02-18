using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Goals.Specifications;

public sealed class UserGoalWithTypeAndDateSpecification(
    UserId userId,
    GoalType type,
    DateOnly date
) : Specification<Goal, GoalId>
{
    public override Expression<Func<Goal, bool>> ToExpression() =>
        goal =>
            goal.UserId == userId
            && goal.Type == type
            && goal.StartDate <= date
            && goal.EndDate >= date;

    public Expression<Func<GoalReadModel, bool>> ToReadModelExpression() =>
        goal =>
            goal.UserId == userId
            && goal.Type == type
            && goal.StartDate <= date
            && goal.EndDate >= date;
}
