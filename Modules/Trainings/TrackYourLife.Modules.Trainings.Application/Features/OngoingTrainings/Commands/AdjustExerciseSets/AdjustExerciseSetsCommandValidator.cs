using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.AdjustExerciseSets;

public class AdjustExerciseSetsCommandValidator : AbstractValidator<AdjustExerciseSetsCommand>
{
    public AdjustExerciseSetsCommandValidator()
    {
        RuleFor(x => x.OngoingTrainingId).NotEmptyId();
        RuleFor(x => x.ExerciseId).NotEmptyId();
        RuleForEach(x => x.ExerciseSetChanges)
            .ChildRules(y =>
            {
                y.RuleFor(z => z.SetId).NotEmpty().NotEqual(Guid.Empty);
                y.RuleFor(z => z.RepsChange).GreaterThanOrEqualTo(0);
                y.RuleFor(z => z.WeightChange).GreaterThanOrEqualTo(0);
            });
    }
}
