using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.DeleteTraining;

public class DeleteTrainingCommandValidator : AbstractValidator<DeleteTrainingCommand>
{
    public DeleteTrainingCommandValidator()
    {
        RuleFor(x => x.TrainingId).NotEmptyId();
    }
}
