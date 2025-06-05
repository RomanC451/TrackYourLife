using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.FinishOngoingTraining;

public sealed class FinishOngoingTrainingCommandValidator
    : AbstractValidator<FinishOngoingTrainingCommand>
{
    public FinishOngoingTrainingCommandValidator()
    {
        RuleFor(command => command.Id).NotEmptyId();
    }
}
