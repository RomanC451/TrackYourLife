using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;

internal sealed record ExerciseDto(
    ExerciseId Id,
    string Name,
    List<string> MuscleGroups,
    Difficulty Difficulty,
    string? PictureUrl,
    string? VideoUrl,
    string? Description,
    string? Equipment,
    IReadOnlyCollection<ExerciseSet> ExerciseSets,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
);
