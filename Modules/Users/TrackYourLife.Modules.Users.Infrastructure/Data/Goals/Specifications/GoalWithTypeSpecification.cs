using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Goals.Specifications;

internal sealed class GoalWithTypeSpecification(UserId userId, GoalType type)
    : Specification<Goal, GoalId>
{
    public override Expression<Func<Goal, bool>> ToExpression() =>
        userGoal => userGoal.UserId == userId && userGoal.Type == type;

    internal Expression<Func<GoalReadModel, bool>> ToReadModelExpression() =>
        goal => goal.UserId == userId && goal.Type == type;
}
