using FluentValidation;

namespace TrackYourLife.Modules.Payments.Application.Features.Checkout.CreateCheckoutSession;

internal sealed class CreateCheckoutSessionCommandValidator
    : AbstractValidator<CreateCheckoutSessionCommand>
{
    public CreateCheckoutSessionCommandValidator()
    {
        RuleFor(x => x.SuccessUrl).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.CancelUrl).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.PriceId).NotEmpty().MaximumLength(256);
    }
}
