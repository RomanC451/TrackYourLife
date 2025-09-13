using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.CreateExercise;

public sealed record CreateExerciseCommand(
    string Name,
    List<string> MuscleGroups,
    Difficulty Difficulty,
    string? PictureUrl,
    string? VideoUrl,
    string? Description,
    string? Equipment,
    List<ExerciseSet> Sets
) : ICommand<ExerciseId>;
