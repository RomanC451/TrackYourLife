using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.UserGoals.Commands.RemoveUserGoal;

public sealed class RemoveUserGoalCommandValidator : AbstractValidator<RemoveUserGoalCommand>
{
    public RemoveUserGoalCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
