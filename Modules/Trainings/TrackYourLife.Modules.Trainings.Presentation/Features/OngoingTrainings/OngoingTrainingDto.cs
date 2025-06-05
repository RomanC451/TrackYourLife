using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings;

internal sealed record OngoingTrainingDto(
    OngoingTrainingId Id,
    TrainingDto Training,
    int ExerciseIndex,
    int SetIndex,
    DateTime StartedOnUtc,
    DateTime? FinishedOnUtc,
    bool HasNext,
    bool HasPrevious,
    bool IsLastSet,
    bool IsFirstSet
);
