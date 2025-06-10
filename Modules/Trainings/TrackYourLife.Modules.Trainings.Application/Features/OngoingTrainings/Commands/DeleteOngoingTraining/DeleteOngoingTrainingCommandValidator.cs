using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.DeleteOngoingTraining;

public class DeleteOngoingTrainingCommandValidator : AbstractValidator<DeleteOngoingTrainingCommand>
{
    public DeleteOngoingTrainingCommandValidator()
    {
        RuleFor(command => command.TrainingId).NotEmptyId();
    }
}
