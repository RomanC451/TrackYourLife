using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories.Specifications;

internal sealed class ExerciseHistoryReadModelWithExerciseIdSpecification(ExerciseId exerciseId)
    : Specification<ExerciseHistoryReadModel, ExerciseHistoryId>
{
    public override Expression<Func<ExerciseHistoryReadModel, bool>> ToExpression() =>
        exerciseHistory => exerciseHistory.ExerciseId == exerciseId;
}
