using System.Text.Json;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;

namespace TrackYourLife.Modules.Payments.FunctionalTests;

internal sealed class MockStripeService : IStripeService
{
    public const string FakeCheckoutUrl = "https://checkout.stripe.com/c/pay/fake-session";
    public const string FakePortalUrl = "https://billing.stripe.com/session/fake-portal";
    public const string FakeCustomerId = "cus_functional_test";

    public Task<bool> CustomerHasActiveSubscriptionForPriceAsync(
        string customerId,
        string priceId,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(false);

    public Task<string> CreateCheckoutSessionAsync(
        string? existingCustomerId,
        string userEmail,
        string userId,
        string priceId,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(FakeCheckoutUrl);

    public Task<string?> GetOrCreateCustomerIdAsync(
        string? existingCustomerId,
        string userEmail,
        string userName,
        CancellationToken cancellationToken = default
    ) => Task.FromResult<string?>(existingCustomerId ?? FakeCustomerId);

    public (StripeWebhookPayload? Payload, string? Error) TryParseWebhookEvent(
        string jsonPayload,
        string signatureHeader
    )
    {
        if (string.IsNullOrWhiteSpace(jsonPayload))
            return (null, "Empty payload");
        var eventType = "checkout.session.completed";
        try
        {
            var doc = JsonDocument.Parse(jsonPayload);
            if (doc.RootElement.TryGetProperty("type", out var typeEl))
                eventType = typeEl.GetString() ?? eventType;
        }
        catch
        {
            // use default
        }

        return (
            new StripeWebhookPayload(
                "evt_functional_test",
                eventType,
                null,
                FakeCustomerId,
                "sub_fake",
                DateTime.UtcNow.AddMonths(1),
                "active",
                false
            ),
            null
        );
    }

    public Task<bool> HasProcessedEventAsync(
        string eventId,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(false);

    public Task MarkEventProcessedAsync(
        string eventId,
        CancellationToken cancellationToken = default
    ) => Task.CompletedTask;

    public Task<string> CreateBillingPortalSessionAsync(
        string customerId,
        string returnUrl,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(FakePortalUrl);
}
