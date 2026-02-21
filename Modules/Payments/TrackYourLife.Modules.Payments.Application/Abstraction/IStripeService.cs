using TrackYourLife.Modules.Payments.Application.Features.Webhooks;

namespace TrackYourLife.Modules.Payments.Application.Abstraction;

public interface IStripeService
{
    Task<bool> CustomerHasActiveSubscriptionForPriceAsync(
        string customerId,
        string priceId,
        CancellationToken cancellationToken = default
    );

    Task<string> CreateCheckoutSessionAsync(
        string? existingCustomerId,
        string userEmail,
        string userId,
        string priceId,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default
    );

    Task<string?> GetOrCreateCustomerIdAsync(
        string? existingCustomerId,
        string userEmail,
        string userName,
        CancellationToken cancellationToken = default
    );

    (StripeWebhookPayload? Payload, string? Error) TryParseWebhookEvent(
        string jsonPayload,
        string signatureHeader
    );

    Task<bool> HasProcessedEventAsync(
        string eventId,
        CancellationToken cancellationToken = default
    );

    Task MarkEventProcessedAsync(string eventId, CancellationToken cancellationToken = default);

    Task<string> CreateBillingPortalSessionAsync(
        string customerId,
        string returnUrl,
        CancellationToken cancellationToken = default
    );
}
