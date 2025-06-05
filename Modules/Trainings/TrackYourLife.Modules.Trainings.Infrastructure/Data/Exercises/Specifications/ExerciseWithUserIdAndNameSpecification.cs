using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Exercises.Specifications;

internal sealed class ExerciseReadModelWithUserIdAndNameSpecification(UserId userId, string name)
    : Specification<ExerciseReadModel, ExerciseId>
{
    public override Expression<Func<ExerciseReadModel, bool>> ToExpression() =>
        exercise => exercise.UserId == userId && exercise.Name == name;
}
