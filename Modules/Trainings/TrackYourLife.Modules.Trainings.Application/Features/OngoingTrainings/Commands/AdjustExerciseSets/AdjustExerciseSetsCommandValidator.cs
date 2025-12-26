using FluentValidation;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Validators;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.AdjustExerciseSets;

public class AdjustExerciseSetsCommandValidator : AbstractValidator<AdjustExerciseSetsCommand>
{
    public AdjustExerciseSetsCommandValidator()
    {
        RuleFor(x => x.OngoingTrainingId).NotEmptyId();
        RuleFor(x => x.ExerciseId).NotEmptyId();
        RuleFor(x => x.NewExerciseSets).NotEmpty();
        RuleForEach(x => x.NewExerciseSets).SetValidator(new ExerciseSetValidator());
    }
}
