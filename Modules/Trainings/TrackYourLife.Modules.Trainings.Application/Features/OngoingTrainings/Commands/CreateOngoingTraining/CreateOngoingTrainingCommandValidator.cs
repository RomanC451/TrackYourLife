using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.CreateOngoingTraining;

public class CreateOngoingTrainingCommandValidator : AbstractValidator<CreateOngoingTrainingCommand>
{
    public CreateOngoingTrainingCommandValidator()
    {
        RuleFor(c => c.TrainingId).NotEmptyId();
    }
}
