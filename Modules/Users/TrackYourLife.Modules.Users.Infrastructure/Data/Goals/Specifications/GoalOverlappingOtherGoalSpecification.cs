using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Goals.Specifications;

internal sealed class GoalOverlappingOtherGoalSpecification(Goal newGoal)
    : Specification<Goal, GoalId>
{
    private readonly Goal _newGoal = newGoal;

    public override Expression<Func<Goal, bool>> ToExpression() =>
        dbGoal =>
            dbGoal.UserId == _newGoal.UserId
            && (
                dbGoal.StartDate <= _newGoal.StartDate && dbGoal.EndDate >= _newGoal.StartDate
                || dbGoal.StartDate <= _newGoal.EndDate && dbGoal.EndDate >= _newGoal.EndDate
                || dbGoal.StartDate >= _newGoal.StartDate && dbGoal.EndDate <= _newGoal.EndDate
            );
}
