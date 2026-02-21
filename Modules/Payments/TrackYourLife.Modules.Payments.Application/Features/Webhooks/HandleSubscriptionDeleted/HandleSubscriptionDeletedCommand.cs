using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionDeleted;

public sealed record HandleSubscriptionDeletedCommand(StripeWebhookPayload Payload)
    : ICommand;
