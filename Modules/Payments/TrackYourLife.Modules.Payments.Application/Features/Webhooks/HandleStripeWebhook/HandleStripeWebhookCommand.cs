using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleStripeWebhook;

public sealed record HandleStripeWebhookCommand(string JsonPayload, string SignatureHeader)
    : ICommand;
