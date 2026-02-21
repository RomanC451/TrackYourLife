using FluentValidation;
using TrackYourLife.SharedLib.Contracts;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateProSubscriptionPeriodEnd;

internal sealed class UpdateProSubscriptionPeriodEndCommandValidator
    : AbstractValidator<UpdateProSubscriptionPeriodEndCommand>
{
    public UpdateProSubscriptionPeriodEndCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PeriodEndUtc).NotEmpty();
        RuleFor(x => x.SubscriptionStatus).IsInEnum();
    }
}
