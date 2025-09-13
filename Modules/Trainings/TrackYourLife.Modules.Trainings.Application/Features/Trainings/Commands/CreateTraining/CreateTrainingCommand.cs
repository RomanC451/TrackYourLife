using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.CreateTraining;

public sealed record CreateTrainingCommand(
    string Name,
    List<string> MuscleGroups,
    Difficulty Difficulty,
    List<ExerciseId> ExercisesIds,
    int Duration,
    int RestSeconds,
    string? Description
) : ICommand<TrainingId>;
