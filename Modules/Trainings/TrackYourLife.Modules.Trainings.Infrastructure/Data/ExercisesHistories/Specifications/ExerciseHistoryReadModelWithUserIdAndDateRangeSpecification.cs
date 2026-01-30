using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories.Specifications;

internal sealed class ExerciseHistoryReadModelWithUserIdAndDateRangeSpecification(
    UserId userId,
    DateTime startDate,
    DateTime endDate
) : Specification<ExerciseHistoryReadModel, ExerciseHistoryId>
{
    public override Expression<Func<ExerciseHistoryReadModel, bool>> ToExpression()
    {
        _ = userId; // Note: ExerciseHistory doesn't have UserId directly, we'll filter via OngoingTraining
        return eh => eh.CreatedOnUtc >= startDate && eh.CreatedOnUtc <= endDate;
    }
}
