using FluentValidation;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Validators;

public class ExerciseSetValidator : AbstractValidator<ExerciseSet>
{
    private const string RestTimeMessage = "Rest time must be greater than or equal to 0";

    public ExerciseSetValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RestTimeSeconds).GreaterThanOrEqualTo(0).WithMessage(RestTimeMessage);

        // Validate WeightBasedExerciseSet
        When(
            x => x.Type == ExerciseSetType.Weight,
            () =>
            {
                RuleFor(x => x.Reps)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Reps must be greater than 0");
                RuleFor(x => x.Weight)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Weight must be greater than 0");
                RuleFor(x => x.DurationSeconds)
                    .Null()
                    .WithMessage("Duration must be equal to 0 when type is Weight");
                RuleFor(x => x.Distance)
                    .Null()
                    .WithMessage("Distance must be equal to 0 when type is Weight");
            }
        );

        // Validate TimeBasedExerciseSet
        When(
            x => x.Type == ExerciseSetType.Time,
            () =>
            {
                RuleFor(x => x.DurationSeconds)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Duration must be greater than zero");
                RuleFor(x => x.Reps)
                    .Null()
                    .WithMessage("Reps must be equal to 0 when type is Time");
                RuleFor(x => x.Weight)
                    .Null()
                    .WithMessage("Weight must be equal to 0 when type is Time");
                RuleFor(x => x.Distance)
                    .Null()
                    .WithMessage("Distance must be equal to 0 when type is Time");
            }
        );

        // Validate BodyweightExerciseSet
        When(
            x => x.Type == ExerciseSetType.Bodyweight,
            () =>
            {
                RuleFor(x => x.Reps)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Reps must be greater than 0");
                RuleFor(x => x.DurationSeconds)
                    .Null()
                    .WithMessage("Duration must be equal to 0 when type is Bodyweight");
                RuleFor(x => x.Weight)
                    .Null()
                    .WithMessage("Weight must be equal to 0 when type is Bodyweight");
                RuleFor(x => x.Distance)
                    .Null()
                    .WithMessage("Distance must be equal to 0 when type is Bodyweight");
            }
        );

        // Validate DistanceExerciseSet
        When(
            x => x.Type == ExerciseSetType.Distance,
            () =>
            {
                RuleFor(x => x.Distance)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Distance must be greater than 0");

                RuleFor(x => x.Reps)
                    .Null()
                    .WithMessage("Reps must be equal to 0 when type is Distance");
                RuleFor(x => x.Weight)
                    .Null()
                    .WithMessage("Weight must be equal to 0 when type is Distance");
                RuleFor(x => x.DurationSeconds)
                    .Null()
                    .WithMessage("Duration must be equal to 0 when type is Distance");
            }
        );
    }
}
