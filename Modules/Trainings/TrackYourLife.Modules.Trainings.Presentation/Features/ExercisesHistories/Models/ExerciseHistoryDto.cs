using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Models;

internal sealed record ExerciseHistoryDto(
    ExerciseHistoryId Id,
    ExerciseId ExerciseId,
    IReadOnlyList<ExerciseSet> NewExerciseSets,
    IReadOnlyList<ExerciseSet> OldExerciseSets,
    bool AreChangesApplied,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
);
