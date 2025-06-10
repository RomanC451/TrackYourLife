using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings;

/// <summary>
/// Represents the extension class for mapping between different types related to trainings.
/// </summary>

internal static class OngoingTrainingMappingsExtensions
{
    public static OngoingTrainingDto ToDto(this OngoingTrainingReadModel ongoingTraining)
    {
        return new OngoingTrainingDto(
            Id: ongoingTraining.Id,
            Training: ongoingTraining.Training.ToDto(),
            ExerciseIndex: ongoingTraining.ExerciseIndex,
            SetIndex: ongoingTraining.SetIndex,
            StartedOnUtc: ongoingTraining.StartedOnUtc,
            FinishedOnUtc: ongoingTraining.FinishedOnUtc,
            HasNext: ongoingTraining.HasNext,
            HasPrevious: ongoingTraining.HasPrevious,
            IsLastSet: ongoingTraining.IsLastSet,
            IsFirstSet: ongoingTraining.IsFirstSet,
            IsLastExercise: ongoingTraining.IsLastExercise,
            IsFirstExercise: ongoingTraining.IsFirstExercise
        );
    }
}
