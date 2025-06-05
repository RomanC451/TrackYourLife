using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.PreviousOngoingTraining;

public class PreviousOngoingTrainingCommandValidator
    : AbstractValidator<PreviousOngoingTrainingCommand>
{
    public PreviousOngoingTrainingCommandValidator()
    {
        RuleFor(c => c.OngoingTrainingId).NotEmptyId();
    }
}
