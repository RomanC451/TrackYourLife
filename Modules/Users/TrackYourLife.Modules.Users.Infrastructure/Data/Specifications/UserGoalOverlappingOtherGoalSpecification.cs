using System.Linq.Expressions;
using TrackYourLife.Common.Persistence.Specifications;
using TrackYourLife.Modules.Users.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Specifications;

internal sealed class UserGoalOverlappingOtherGoalSpecification(UserGoal newGoal)
        : Specification<UserGoal, UserGoalId>
{
    private readonly UserGoal _newGoal = newGoal;

    public override Expression<Func<UserGoal, bool>> ToExpression() =>
        dbGoal =>
            dbGoal.UserId == _newGoal.UserId
            && (
                dbGoal.StartDate <= _newGoal.StartDate && dbGoal.EndDate >= _newGoal.StartDate
                || dbGoal.StartDate <= _newGoal.EndDate && dbGoal.EndDate >= _newGoal.EndDate
                || dbGoal.StartDate >= _newGoal.StartDate && dbGoal.EndDate <= _newGoal.EndDate
            );
}
