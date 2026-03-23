using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.UpdateOngoingTraining;

public sealed class UpdateOngoingTrainingCommandValidator
    : AbstractValidator<UpdateOngoingTrainingCommand>
{
    public UpdateOngoingTrainingCommandValidator()
    {
        RuleFor(command => command.Id).NotEmptyId();
        RuleFor(command => command.CaloriesBurned).GreaterThanOrEqualTo(0);
        RuleFor(command => command.DurationMinutes).GreaterThanOrEqualTo(0);
    }
}
