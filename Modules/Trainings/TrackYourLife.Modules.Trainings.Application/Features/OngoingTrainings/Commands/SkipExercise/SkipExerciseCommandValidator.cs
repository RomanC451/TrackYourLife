using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.SkipExercise;

public class SkipExerciseCommandValidator : AbstractValidator<SkipExerciseCommand>
{
    public SkipExerciseCommandValidator()
    {
        RuleFor(c => c.OngoingTrainingId).NotEmptyId();
    }
}
