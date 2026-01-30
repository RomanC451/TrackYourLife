using FluentValidation;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExercisePerformance;

public class GetExercisePerformanceQueryValidator : AbstractValidator<GetExercisePerformanceQuery>
{
    public GetExercisePerformanceQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be less than or equal to end date");

        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must not exceed 100");

        RuleFor(x => x.CalculationMethod)
            .IsInEnum()
            .WithMessage("CalculationMethod must be a valid enum value");
    }
}
