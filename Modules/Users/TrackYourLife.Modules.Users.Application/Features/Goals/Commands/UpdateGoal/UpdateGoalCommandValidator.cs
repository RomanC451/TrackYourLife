using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;

public sealed class UpdateGoalCommandValidator : AbstractValidator<UpdateGoalCommand>
{
    public UpdateGoalCommandValidator()
    {
        RuleFor(x => x.Id).NotEmptyId();

        RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid goal type.");

        RuleFor(x => x.Value).NotEmpty().GreaterThan(0);

        RuleFor(x => x.PerPeriod).IsInEnum().WithMessage("Invalid goal period.");

        RuleFor(x => x.StartDate).NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate is not null);

        RuleFor(x => x.Force).NotNull();
    }
}
