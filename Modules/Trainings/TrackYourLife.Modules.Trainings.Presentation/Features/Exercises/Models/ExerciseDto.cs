using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Sets;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;

internal sealed record ExerciseDto(
    ExerciseId Id,
    string Name,
    string? PictureUrl,
    string? VideoUrl,
    string? Description,
    string? Equipment,
    IReadOnlyCollection<ExerciseSet> ExerciseSets,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
);
