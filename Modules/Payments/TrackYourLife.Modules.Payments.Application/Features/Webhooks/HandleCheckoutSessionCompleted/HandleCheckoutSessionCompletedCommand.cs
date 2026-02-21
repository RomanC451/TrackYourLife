using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleCheckoutSessionCompleted;

public sealed record HandleCheckoutSessionCompletedCommand(StripeWebhookPayload Payload) : ICommand;
