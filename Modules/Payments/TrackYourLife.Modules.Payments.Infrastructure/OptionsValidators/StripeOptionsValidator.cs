using FluentValidation;
using TrackYourLife.Modules.Payments.Infrastructure.Options;

namespace TrackYourLife.Modules.Payments.Infrastructure.OptionsValidators;

internal sealed class StripeOptionsValidator : AbstractValidator<StripeOptions>
{
    public StripeOptionsValidator()
    {
        RuleFor(x => x.WebhookSecret).NotEmpty();
        RuleFor(x => x.ProPriceIdMonthly).NotEmpty();
        RuleFor(x => x.ProPriceIdYearly).NotEmpty();
    }
}
