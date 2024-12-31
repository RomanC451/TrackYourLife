using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;

public sealed class UpdateGoalCommandValidator : AbstractValidator<UpdateGoalCommand>
{
    public UpdateGoalCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Value).NotEmpty();

        RuleFor(x => x.StartDate).NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate is not null);
    }
}
