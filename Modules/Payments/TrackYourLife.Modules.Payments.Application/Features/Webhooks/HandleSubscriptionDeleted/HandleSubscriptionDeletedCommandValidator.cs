using FluentValidation;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionDeleted;

internal sealed class HandleSubscriptionDeletedCommandValidator
    : AbstractValidator<HandleSubscriptionDeletedCommand>
{
    public HandleSubscriptionDeletedCommandValidator()
    {
        RuleFor(x => x.Payload).NotNull();
    }
}
