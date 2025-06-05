using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Trainings.Specifications;

public class TrainingWithExerciseSpecification(ExerciseId exerciseId)
    : Specification<Training, TrainingId>
{
    public override Expression<Func<Training, bool>> ToExpression() =>
        training => training.TrainingExercises.Any(e => e.Exercise.Id == exerciseId);
}
