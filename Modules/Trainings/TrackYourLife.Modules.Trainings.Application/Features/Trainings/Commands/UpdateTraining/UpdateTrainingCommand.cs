using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.UpdateTraining;

public sealed record UpdateTrainingCommand(
    TrainingId TrainingId,
    string Name,
    int Duration,
    string? Description,
    IReadOnlyList<ExerciseId> ExerciseIds
) : ICommand;
