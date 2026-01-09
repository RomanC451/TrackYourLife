using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories.Specifications;

internal sealed class ExerciseHistoryWithOngoingTrainingIdSpecification(
    OngoingTrainingId ongoingTrainingId
) : Specification<ExerciseHistory, ExerciseHistoryId>
{
    public override Expression<Func<ExerciseHistory, bool>> ToExpression() =>
        exerciseHistory => exerciseHistory.OngoingTrainingId == ongoingTrainingId;
}
