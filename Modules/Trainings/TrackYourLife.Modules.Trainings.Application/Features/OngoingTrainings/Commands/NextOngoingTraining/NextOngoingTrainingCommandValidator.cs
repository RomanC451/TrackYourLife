using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.NextOngoingTraining;

public class NextOngoingTrainingCommandValidator : AbstractValidator<NextOngoingTrainingCommand>
{
    public NextOngoingTrainingCommandValidator()
    {
        RuleFor(c => c.OngoingTrainingId).NotEmptyId();
    }
}
