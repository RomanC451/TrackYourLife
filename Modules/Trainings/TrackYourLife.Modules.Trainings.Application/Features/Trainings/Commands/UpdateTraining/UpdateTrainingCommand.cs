using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.UpdateTraining;

public sealed record UpdateTrainingCommand(
    TrainingId TrainingId,
    string Name,
    List<string> MuscleGroups,
    Difficulty Difficulty,
    int Duration,
    int RestSeconds,
    string? Description,
    IReadOnlyList<ExerciseId> ExerciseIds
) : ICommand;
