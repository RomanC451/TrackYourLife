using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

public sealed record OngoingTrainingReadModel(
    OngoingTrainingId Id,
    UserId UserId,
    int ExerciseIndex,
    int SetIndex,
    DateTime StartedOnUtc,
    DateTime? FinishedOnUtc,
    int? CaloriesBurned
) : IReadModel<OngoingTrainingId>
{
    public TrainingReadModel Training { get; set; } = null!;

    public int ExercisesCount => Training.TrainingExercises.Count;

    public ExerciseReadModel CurrentExercise =>
        Training.TrainingExercises.OrderBy(te => te.OrderIndex).ElementAt(ExerciseIndex).Exercise;

    public int SetsCount => CurrentExercise.ExerciseSets.Count;

    public bool IsFinished => FinishedOnUtc.HasValue;
    public bool IsLastSet => SetIndex == SetsCount - 1;
    public bool IsLastExercise => ExerciseIndex == ExercisesCount - 1;
    public bool IsLastSetAndExercise => IsLastSet && IsLastExercise;

    public bool HasNext(IReadOnlySet<ExerciseId> completedOrSkippedExerciseIds)
    {
        // If we're at the last set of the last exercise, there's no next position
        if (IsLastSetAndExercise)
        {
            return false;
        }

        var orderedExercises = Training.TrainingExercises.OrderBy(te => te.OrderIndex).ToList();
        var allExerciseIds = orderedExercises.Select(te => te.ExerciseId).ToHashSet();
        var incompleteExerciseIds = allExerciseIds
            .Except(completedOrSkippedExerciseIds)
            .ToHashSet();
        return incompleteExerciseIds.Any();
    }

    public bool IsFirstSet => SetIndex == 0;
    public bool IsFirstExercise => ExerciseIndex == 0;
    public bool IsFirstSetAndExercise => IsFirstSet && IsFirstExercise;
}
