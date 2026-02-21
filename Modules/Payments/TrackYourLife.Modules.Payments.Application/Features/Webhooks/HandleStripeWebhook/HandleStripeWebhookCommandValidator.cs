using FluentValidation;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleStripeWebhook;

internal sealed class HandleStripeWebhookCommandValidator
    : AbstractValidator<HandleStripeWebhookCommand>
{
    public HandleStripeWebhookCommandValidator()
    {
        RuleFor(x => x.JsonPayload).NotEmpty();
        RuleFor(x => x.SignatureHeader).NotEmpty();
    }
}
