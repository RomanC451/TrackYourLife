using System.Linq.Expressions;
using TrackYourLife.Common.Persistence.Specifications;
using TrackYourLife.Modules.Users.Domain.UserGoals;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Specifications;

internal sealed class UserGoalWithTypeSpecification(UserId userId, UserGoalType type) : Specification<UserGoal, UserGoalId>
{
    private readonly UserId _userId = userId;
    private readonly UserGoalType _type = type;

    public override Expression<Func<UserGoal, bool>> ToExpression() =>
        userGoal =>
            userGoal.UserId == _userId
            && userGoal.Type == _type;
}
