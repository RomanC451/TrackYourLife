using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionUpdated;

public sealed record HandleSubscriptionUpdatedCommand(StripeWebhookPayload Payload) : ICommand;
