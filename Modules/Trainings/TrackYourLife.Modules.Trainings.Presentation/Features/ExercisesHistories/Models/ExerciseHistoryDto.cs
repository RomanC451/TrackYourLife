using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Models;

internal sealed record ExerciseHistoryDto(
    ExerciseHistoryId Id,
    ExerciseId ExerciseId,
    IReadOnlyList<ExerciseSetChange> ExerciseSetChanges,
    IReadOnlyList<ExerciseSet> ExerciseSetsBeforeChange,
    bool AreChangesApplied,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
);
