namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;

internal sealed record ExerciseSetDto(
    Guid? Id,
    string Name,
    int Reps,
    float Weight,
    int OrderIndex
);
