using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.WorkoutPlans.Specifications;

internal sealed class WorkoutPlanWithUserIdSpecification(UserId userId)
    : Specification<WorkoutPlan, WorkoutPlanId>
{
    public override Expression<Func<WorkoutPlan, bool>> ToExpression() => wp => wp.UserId == userId;
}
