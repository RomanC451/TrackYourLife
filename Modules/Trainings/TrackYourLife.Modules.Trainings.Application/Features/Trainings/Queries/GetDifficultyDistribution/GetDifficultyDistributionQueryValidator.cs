using FluentValidation;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetDifficultyDistribution;

public class GetDifficultyDistributionQueryValidator
    : AbstractValidator<GetDifficultyDistributionQuery>
{
    public GetDifficultyDistributionQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be less than or equal to end date");
    }
}
