using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Exercises.Specifications;

internal sealed class ExerciseReadModelWithinIdsCollectionSpecification(IEnumerable<ExerciseId> ids)
    : Specification<ExerciseReadModel, ExerciseId>
{
    public override Expression<Func<ExerciseReadModel, bool>> ToExpression() =>
        exercise => ids.Contains(exercise.Id);
}

internal sealed class ExerciseWithinIdsCollectionSpecification(IEnumerable<ExerciseId> ids)
    : Specification<Exercise, ExerciseId>
{
    public override Expression<Func<Exercise, bool>> ToExpression() =>
        exercise => ids.Contains(exercise.Id);
}
