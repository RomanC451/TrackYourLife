using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.DeleteExercise;

public class DeleteExerciseCommandValidator : AbstractValidator<DeleteExerciseCommand>
{
    public DeleteExerciseCommandValidator()
    {
        RuleFor(x => x.ExerciseId).NotEmptyId();
    }
}
