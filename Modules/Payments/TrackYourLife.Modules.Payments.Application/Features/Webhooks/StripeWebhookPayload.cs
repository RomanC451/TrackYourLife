namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks;

public sealed record StripeWebhookPayload(
    string EventId,
    string EventType,
    string? ClientReferenceId,
    string? CustomerId,
    string? SubscriptionId,
    DateTime? CurrentPeriodEndUtc,
    string? SubscriptionStatus,
    bool CancelAtPeriodEnd = false
);
