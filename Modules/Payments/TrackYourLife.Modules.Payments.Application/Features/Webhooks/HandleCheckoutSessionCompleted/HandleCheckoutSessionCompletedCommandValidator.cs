using FluentValidation;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleCheckoutSessionCompleted;

internal sealed class HandleCheckoutSessionCompletedCommandValidator
    : AbstractValidator<HandleCheckoutSessionCompletedCommand>
{
    public HandleCheckoutSessionCompletedCommandValidator()
    {
        RuleFor(x => x.Payload).NotNull();

        RuleFor(x => x.Payload!.ClientReferenceId)
            .NotEmpty()
            .Must(s => Guid.TryParse(s, out _))
            .WithMessage("Missing or invalid client_reference_id.")
            .When(x => x.Payload != null);

        RuleFor(x => x.Payload!.CurrentPeriodEndUtc)
            .NotNull()
            .WithMessage("Missing subscription or customer data.")
            .When(x => x.Payload != null);

        RuleFor(x => x.Payload!.CustomerId)
            .NotEmpty()
            .WithMessage("Missing subscription or customer data.")
            .When(x => x.Payload != null);
    }
}
