using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.WorkoutPlans.Specifications;

internal sealed class WorkoutPlanReadModelWithUserIdSpecification(UserId userId)
    : Specification<WorkoutPlanReadModel, WorkoutPlanId>
{
    public override Expression<Func<WorkoutPlanReadModel, bool>> ToExpression() =>
        wp => wp.UserId == userId;
}
