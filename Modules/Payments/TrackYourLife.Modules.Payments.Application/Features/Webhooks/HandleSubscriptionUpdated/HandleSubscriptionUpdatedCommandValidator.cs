using FluentValidation;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionUpdated;

internal sealed class HandleSubscriptionUpdatedCommandValidator
    : AbstractValidator<HandleSubscriptionUpdatedCommand>
{
    public HandleSubscriptionUpdatedCommandValidator()
    {
        RuleFor(x => x.Payload).NotNull();
    }
}
