using System.Linq.Expressions;
using TrackYourLife.Common.Persistence.Specifications;
using TrackYourLife.Modules.Users.Domain.UserGoals;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;


namespace TrackYourLife.Modules.Users.Infrastructure.Data.Specifications;

internal class ActiveUserGoalSpecification(UserId userId, UserGoalType type) : Specification<UserGoal, UserGoalId>
{
    private readonly UserId _userId = userId;
    private readonly UserGoalType _type = type;

    public override Expression<Func<UserGoal, bool>> ToExpression() =>
        goal => goal.UserId == _userId && goal.Type == _type && goal.EndDate == DateOnly.MaxValue;


    internal Expression<Func<UserGoalReadModel, bool>> ToReadModelExpression() =>
        goal =>
            goal.UserId == _userId.Value && goal.Type == _type && goal.EndDate == DateOnly.MaxValue;
}
