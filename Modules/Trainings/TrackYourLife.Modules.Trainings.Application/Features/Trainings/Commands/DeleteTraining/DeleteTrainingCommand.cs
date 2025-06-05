using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.DeleteTraining;

public sealed record DeleteTrainingCommand(TrainingId TrainingId) : ICommand;
