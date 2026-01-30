using FluentValidation;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsOverview;

public class GetTrainingsOverviewQueryValidator : AbstractValidator<GetTrainingsOverviewQuery>
{
    public GetTrainingsOverviewQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be less than or equal to end date");
    }
}
