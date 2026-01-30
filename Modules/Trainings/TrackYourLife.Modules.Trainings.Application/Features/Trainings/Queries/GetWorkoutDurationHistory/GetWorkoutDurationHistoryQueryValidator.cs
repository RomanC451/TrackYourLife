using FluentValidation;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutDurationHistory;

public class GetWorkoutDurationHistoryQueryValidator
    : AbstractValidator<GetWorkoutDurationHistoryQuery>
{
    public GetWorkoutDurationHistoryQueryValidator()
    {
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("End date must be greater than or equal to start date");
    }
}
