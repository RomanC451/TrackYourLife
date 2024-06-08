using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.UserGoals.Commands.UpdateUserGoal;

public sealed class UpdateUserGoalCommandValidator : AbstractValidator<UpdateUserGoalCommand>
{
    public UpdateUserGoalCommandValidator()
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
