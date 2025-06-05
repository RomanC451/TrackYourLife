using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Exercises.Specifications;

internal sealed class ExerciseReadModelWithUserIdSpecification(UserId userId)
    : Specification<ExerciseReadModel, ExerciseId>
{
    public override Expression<Func<ExerciseReadModel, bool>> ToExpression() =>
        exercise => exercise.UserId == userId;
}

internal sealed class ExerciseWithUserIdSpecification(UserId userId)
    : Specification<Exercise, ExerciseId>
{
    public override Expression<Func<Exercise, bool>> ToExpression() =>
        exercise => exercise.UserId == userId;
}
