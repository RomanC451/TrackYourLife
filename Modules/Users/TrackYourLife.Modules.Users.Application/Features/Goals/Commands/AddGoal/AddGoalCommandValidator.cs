using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.AddGoal;

public sealed class AddGoalCommandValidator : AbstractValidator<AddGoalCommand>
{
    public AddGoalCommandValidator()
    {
        RuleFor(x => x.Value).NotEmpty();

        RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid goal type.");

        RuleFor(x => x.PerPeriod).IsInEnum().WithMessage("Invalid goal period.");

        RuleFor(x => x.StartDate).NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate is not null);

        RuleFor(x => x.Force).NotNull();
    }
}
