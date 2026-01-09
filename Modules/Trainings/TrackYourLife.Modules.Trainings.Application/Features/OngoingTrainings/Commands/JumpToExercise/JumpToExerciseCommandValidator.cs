using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.JumpToExercise;

public class JumpToExerciseCommandValidator : AbstractValidator<JumpToExerciseCommand>
{
    public JumpToExerciseCommandValidator()
    {
        RuleFor(c => c.OngoingTrainingId).NotEmptyId();
        RuleFor(c => c.ExerciseIndex).GreaterThanOrEqualTo(0);
    }
}
