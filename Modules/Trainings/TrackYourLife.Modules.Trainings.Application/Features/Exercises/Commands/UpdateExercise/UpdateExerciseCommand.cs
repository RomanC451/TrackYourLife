using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.UpdateExercise;

public sealed record UpdateExerciseCommand(
    ExerciseId Id,
    string Name,
    List<string> MuscleGroups,
    Difficulty Difficulty,
    string? Description,
    string? VideoUrl,
    string? PictureUrl,
    string? Equipment,
    List<ExerciseSet> ExerciseSets
) : ICommand;
