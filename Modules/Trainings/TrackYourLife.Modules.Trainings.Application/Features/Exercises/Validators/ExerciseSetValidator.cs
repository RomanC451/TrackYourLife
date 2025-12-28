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

        // Validate Count1 and Unit1 (required)
        RuleFor(x => x.Count1).GreaterThan(0).WithMessage("Count1 must be greater than 0");

        RuleFor(x => x.Unit1).NotEmpty().WithMessage("Unit1 is required");

        // Validate Count2 and Unit2 consistency (both must be provided or both must be null/empty)
        When(
            x => x.Count2.HasValue,
            () =>
            {
                RuleFor(x => x.Unit2)
                    .NotEmpty()
                    .WithMessage("Unit2 must be provided when Count2 is provided");
                RuleFor(x => x.Count2)
                    .GreaterThan(0)
                    .WithMessage("Count2 must be greater than 0 when provided");
            }
        );

        When(
            x => !string.IsNullOrEmpty(x.Unit2),
            () =>
            {
                RuleFor(x => x.Count2)
                    .NotNull()
                    .WithMessage("Count2 must be provided when Unit2 is provided");
            }
        );
    }
}
