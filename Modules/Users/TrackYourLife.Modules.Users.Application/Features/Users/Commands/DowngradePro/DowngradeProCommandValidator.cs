using FluentValidation;
using TrackYourLife.SharedLib.Contracts;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.DowngradePro;

internal sealed class DowngradeProCommandValidator : AbstractValidator<DowngradeProCommand>
{
    public DowngradeProCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SubscriptionStatus).IsInEnum();
    }
}
