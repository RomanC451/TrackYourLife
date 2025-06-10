using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.DeleteOngoingTraining;

public sealed record DeleteOngoingTrainingCommand(TrainingId TrainingId) : ICommand;
