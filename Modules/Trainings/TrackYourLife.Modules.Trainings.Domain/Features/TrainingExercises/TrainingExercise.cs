using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;

public sealed class TrainingExercise
{
    public TrainingId TrainingId { get; } = TrainingId.Empty;

    public ExerciseId ExerciseId { get; } = ExerciseId.Empty;

    public Exercise Exercise { get; set; } = null!;

    public int OrderIndex { get; }

    private TrainingExercise() { }

    private TrainingExercise(TrainingId trainingId, Exercise exercise, int orderIndex)
    {
        TrainingId = trainingId;
        Exercise = exercise;
        OrderIndex = orderIndex;
    }

    public static Result<TrainingExercise> Create(
        TrainingId trainingId,
        Exercise exercise,
        int orderIndex
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                trainingId,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(trainingId))
            ),
            Ensure.NotNull(
                exercise,
                DomainErrors.ArgumentError.Null(nameof(Training), nameof(exercise))
            ),
            Ensure.NotNegative(
                orderIndex,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(orderIndex))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<TrainingExercise>(result.Error);
        }

        return Result.Success(new TrainingExercise(trainingId, exercise, orderIndex));
    }
}
