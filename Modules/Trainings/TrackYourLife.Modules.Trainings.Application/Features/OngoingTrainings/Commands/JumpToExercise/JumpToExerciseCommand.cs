using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.JumpToExercise;

public sealed record JumpToExerciseCommand(OngoingTrainingId OngoingTrainingId, int ExerciseIndex)
    : ICommand;
