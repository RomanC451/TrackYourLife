using FluentValidation;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetTopExercises;

public class GetTopExercisesQueryValidator : AbstractValidator<GetTopExercisesQuery>
{
    public GetTopExercisesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than 0");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be less than or equal to end date");
    }
}
