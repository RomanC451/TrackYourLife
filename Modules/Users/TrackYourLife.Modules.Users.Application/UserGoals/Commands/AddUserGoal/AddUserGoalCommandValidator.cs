using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.UserGoals.Commands.AddUserGoal;

public sealed class AddUserGoalCommandValidator : AbstractValidator<AddUserGoalCommand>
{
    public AddUserGoalCommandValidator()
    {
        RuleFor(x => x.Value).NotEmpty();

        RuleFor(x => x.StartDate).NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate is not null);
    }
}
