using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings;

/// <summary>
/// Represents the extension class for mapping between different types related to trainings.
/// </summary>

internal static class OngoingTrainingMappingsExtensions
{
    public static OngoingTrainingDto ToDto(
        this OngoingTrainingReadModel ongoingTraining,
        IEnumerable<ExerciseHistoryReadModel>? exerciseHistories = null
    )
    {
        var completedExerciseIds =
            exerciseHistories
                ?.Where(eh => eh.Status == ExerciseStatus.Completed)
                .Select(eh => eh.ExerciseId)
                .ToList() ?? new List<ExerciseId>();

        var skippedExerciseIds =
            exerciseHistories
                ?.Where(eh => eh.Status == ExerciseStatus.Skipped)
                .Select(eh => eh.ExerciseId)
                .ToList() ?? new List<ExerciseId>();

        var completedOrSkippedExerciseIds = completedExerciseIds
            .Concat(skippedExerciseIds)
            .ToHashSet();

        var hasNext = ongoingTraining.HasNext(completedOrSkippedExerciseIds);

        return new OngoingTrainingDto(
            Id: ongoingTraining.Id,
            Training: ongoingTraining.Training.ToDto(),
            ExerciseIndex: ongoingTraining.ExerciseIndex,
            SetIndex: ongoingTraining.SetIndex,
            StartedOnUtc: ongoingTraining.StartedOnUtc,
            FinishedOnUtc: ongoingTraining.FinishedOnUtc,
            HasNext: hasNext,
            IsLastSet: ongoingTraining.IsLastSet,
            IsFirstSet: ongoingTraining.IsFirstSet,
            IsLastExercise: ongoingTraining.IsLastExercise,
            IsFirstExercise: ongoingTraining.IsFirstExercise,
            CompletedExerciseIds: completedExerciseIds,
            SkippedExerciseIds: skippedExerciseIds
        );
    }
}
