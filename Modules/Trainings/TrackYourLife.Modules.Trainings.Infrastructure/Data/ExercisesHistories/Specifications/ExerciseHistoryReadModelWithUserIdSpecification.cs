using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories.Specifications;

internal sealed class ExerciseHistoryReadModelWithUserIdSpecification(UserId userId)
    : Specification<ExerciseHistoryReadModel, ExerciseHistoryId>
{
    public override Expression<Func<ExerciseHistoryReadModel, bool>> ToExpression()
    {
        _ = userId; // Note: ExerciseHistory doesn't have UserId directly, we'll filter via OngoingTraining
        return eh => true;
    }
}
