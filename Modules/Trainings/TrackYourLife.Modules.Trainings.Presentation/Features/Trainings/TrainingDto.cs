using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;

internal sealed record TrainingDto(
    TrainingId Id,
    string Name,
    List<string> MuscleGroups,
    Difficulty Difficulty,
    int Duration,
    int RestSeconds,
    string? Description,
    IReadOnlyCollection<ExerciseDto> Exercises,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
);
