using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.AdjustExerciseSets;

public class AdjustExerciseSetsCommandValidator : AbstractValidator<AdjustExerciseSetsCommand>
{
    public AdjustExerciseSetsCommandValidator()
    {
        RuleFor(x => x.OngoingTrainingId).NotEmptyId();
        RuleFor(x => x.ExerciseId).NotEmptyId();
        RuleFor(x => x.ExerciseSetChanges).NotEmpty();
        RuleForEach(x => x.ExerciseSetChanges)
            .ChildRules(y =>
            {
                y.RuleFor(z => z.SetId).NotEmpty().NotEqual(Guid.Empty);
                y.RuleFor(z => z.RepsChange).NotNull();
                y.RuleFor(z => z.WeightChange).NotNull();
            });
    }
}
