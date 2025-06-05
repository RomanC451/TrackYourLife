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
    DateTime? FinishedOnUtc
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
    public bool HasNext => !IsLastSetAndExercise;
    public bool IsFirstSet => SetIndex == 0;
    public bool IsFirstExercise => ExerciseIndex == 0;
    public bool IsFirstSetAndExercise => IsFirstSet && IsFirstExercise;
    public bool HasPrevious => !IsFirstSetAndExercise;
}
