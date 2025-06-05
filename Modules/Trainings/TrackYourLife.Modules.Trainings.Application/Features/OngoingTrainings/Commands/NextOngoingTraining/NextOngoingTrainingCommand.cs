using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.NextOngoingTraining;

public sealed record NextOngoingTrainingCommand(OngoingTrainingId OngoingTrainingId) : ICommand;
