using System.Text.Json;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Contracts;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;

namespace TrackYourLife.Modules.Payments.FunctionalTests;

internal sealed class MockStripeService : IStripeService
{
    public const string FakeCheckoutUrl = "https://checkout.stripe.com/c/pay/fake-session";
    public const string FakePortalUrl = "https://billing.stripe.com/session/fake-portal";
    public const string FakeCustomerId = "cus_functional_test";

    public Task<BillingSummaryDto> GetBillingSummaryAsync(
        string customerId,
        CancellationToken cancellationToken = default
    ) =>
        Task.FromResult(
            new BillingSummaryDto(
                Subscription: new SubscriptionSummaryDto(
                    PlanName: "Pro",
                    UnitAmount: 19,
                    Currency: "USD",
                    Interval: "month",
                    Status: "active",
                    CurrentPeriodEndUtc: DateTime.UtcNow.AddMonths(1),
                    CancelAtPeriodEnd: false
                ),
                PaymentMethod: new PaymentMethodSummaryDto(
                    Brand: "Visa",
                    Last4: "4242",
                    ExpMonth: 8,
                    ExpYear: 2027,
                    BillingName: "Functional Test",
                    IsExpiringSoon: false
                ),
                BillingDetails: new BillingDetailsSummaryDto(
                    BillingAddress: new BillingAddressDto(
                        Line1: "123 Main Street",
                        Line2: null,
                        City: "Bucharest",
                        State: "Sector 1",
                        PostalCode: "010101",
                        Country: "RO"
                    ),
                    CompanyName: "Track Your Life SRL",
                    VatId: "RO12345678"
                ),
                Invoices:
                [
                    new InvoiceSummaryDto(
                        Id: "inv_test_1",
                        CreatedUtc: DateTime.UtcNow.AddDays(-30),
                        Amount: 19,
                        Currency: "USD",
                        Status: "paid",
                        HostedInvoiceUrl: "https://stripe.com/invoice/test",
                        InvoicePdf: null,
                        ReceiptUrl: "https://stripe.com/receipt/test"
                    ),
                ]
            )
        );

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
