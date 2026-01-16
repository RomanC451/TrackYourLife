using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.FinishOngoingTraining;

public sealed record FinishOngoingTrainingCommand(OngoingTrainingId Id, int? CaloriesBurned = null) : ICommand;
