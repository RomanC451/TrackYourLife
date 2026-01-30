using FluentValidation;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutFrequency;

public class GetWorkoutFrequencyQueryValidator : AbstractValidator<GetWorkoutFrequencyQuery>
{
    public GetWorkoutFrequencyQueryValidator()
    {
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("End date must be greater than or equal to start date");
    }
}
