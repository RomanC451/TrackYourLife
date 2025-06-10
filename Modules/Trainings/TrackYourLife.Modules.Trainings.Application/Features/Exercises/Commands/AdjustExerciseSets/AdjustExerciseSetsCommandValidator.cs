using FluentValidation;
using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.AdjustExerciseSets;

public class AdjustExerciseSetsCommandValidator : AbstractValidator<AdjustExerciseSetsCommand>
{
    public AdjustExerciseSetsCommandValidator()
    {
        RuleFor(x => x.ExerciseId).NotEmptyId();
        RuleFor(x => x.ExerciseSetChanges).NotEmpty();
    }
}
