using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpgradeToPro;

internal sealed class UpgradeToProCommandValidator : AbstractValidator<UpgradeToProCommand>
{
    public UpgradeToProCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.StripeCustomerId).NotEmpty().MaximumLength(256);
        RuleFor(x => x.PeriodEndUtc).NotEmpty();
    }
}
