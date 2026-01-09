using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Models;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories;

internal static class ExerciseHistoryMappingsExtensions
{
    public static ExerciseHistoryDto ToDto(this ExerciseHistoryReadModel exerciseHistory)
    {
        return new ExerciseHistoryDto(
            Id: exerciseHistory.Id,
            ExerciseId: exerciseHistory.ExerciseId,
            NewExerciseSets: exerciseHistory.NewExerciseSets,
            OldExerciseSets: exerciseHistory.OldExerciseSets,
            AreChangesApplied: exerciseHistory.AreChangesApplied,
            Status: exerciseHistory.Status,
            CreatedOnUtc: exerciseHistory.CreatedOnUtc,
            ModifiedOnUtc: exerciseHistory.ModifiedOnUtc
        );
    }
}
