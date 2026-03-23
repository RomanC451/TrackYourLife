using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.UpdateOngoingTraining;

public sealed record UpdateOngoingTrainingCommand(
    OngoingTrainingId Id,
    int CaloriesBurned,
    int DurationMinutes
) : ICommand;
