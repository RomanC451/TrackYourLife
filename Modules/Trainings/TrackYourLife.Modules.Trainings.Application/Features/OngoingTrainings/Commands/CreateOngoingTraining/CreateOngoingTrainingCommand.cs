using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.CreateOngoingTraining;

public sealed record CreateOngoingTrainingCommand(TrainingId TrainingId) : ICommand<OngoingTrainingId>;
