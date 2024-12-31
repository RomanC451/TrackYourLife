using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.RemoveGoal;

public sealed class RemoveGoalCommandValidator : AbstractValidator<RemoveGoalCommand>
{
    public RemoveGoalCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
