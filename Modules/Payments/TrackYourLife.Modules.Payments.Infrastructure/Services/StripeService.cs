using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;
using TrackYourLife.Modules.Payments.Infrastructure.Options;

namespace TrackYourLife.Modules.Payments.Infrastructure.Services;

internal sealed class StripeService : IStripeService
{
    private static readonly ConcurrentDictionary<string, byte> ProcessedEvents = new();

    private readonly StripeOptions _options;

    public StripeService(IOptions<StripeOptions> options)
    {
        _options = options.Value;
        StripeConfiguration.ApiKey = _options.SecretKey;
    }

    public async Task<string?> GetOrCreateCustomerIdAsync(
        string? existingCustomerId,
        string userEmail,
        string userName,
        CancellationToken cancellationToken = default
    )
    {
        if (!string.IsNullOrEmpty(existingCustomerId))
        {
            return existingCustomerId;
        }

        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(
            new CustomerCreateOptions { Email = userEmail, Name = userName },
            cancellationToken: cancellationToken
        );

        return customer.Id;
    }

    public async Task<bool> CustomerHasActiveSubscriptionForPriceAsync(
        string customerId,
        string priceId,
        CancellationToken cancellationToken = default
    )
    {
        var resolvedPriceId = ResolvePriceId(priceId);
        if (string.IsNullOrEmpty(resolvedPriceId))
        {
            return false;
        }

        var subscriptionService = new SubscriptionService();
        var listOptions = new SubscriptionListOptions
        {
            Customer = customerId,
            Price = resolvedPriceId,
            Status = "active",
            Limit = 1,
        };
        var subscriptions = await subscriptionService.ListAsync(
            listOptions,
            cancellationToken: cancellationToken
        );
        return subscriptions.Data.Count > 0;
    }

    public async Task<string> CreateCheckoutSessionAsync(
        string? existingCustomerId,
        string userEmail,
        string userId,
        string priceId,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default
    )
    {
        var resolvedPriceId = ResolvePriceId(priceId);
        if (string.IsNullOrEmpty(resolvedPriceId))
        {
            throw new InvalidOperationException(
                "No Stripe Price ID configured. Set ProPriceIdMonthly (and optionally ProPriceIdYearly) in Stripe configuration, or pass a valid Stripe Price ID from the client."
            );
        }

        var customerId = await GetOrCreateCustomerIdAsync(
            existingCustomerId,
            userEmail,
            userEmail,
            cancellationToken
        );

        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(
            new SessionCreateOptions
            {
                Customer = customerId,
                Mode = "subscription",
                ClientReferenceId = userId,
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                LineItems = [new SessionLineItemOptions { Price = resolvedPriceId, Quantity = 1 }],
            },
            cancellationToken: cancellationToken
        );

        return session.Url ?? throw new InvalidOperationException("Stripe session URL was null.");
    }

    /// <summary>
    /// Resolves placeholder price IDs from the client to configured Stripe Price IDs.
    /// When the frontend does not set VITE_STRIPE_PRICE_ID_MONTHLY, it sends "price_monthly";
    /// we map that to the server-configured ProPriceIdMonthly.
    /// </summary>
    internal string? ResolvePriceId(string? priceId)
    {
        if (string.IsNullOrWhiteSpace(priceId))
            return _options.ProPriceIdMonthly;

        if (string.Equals(priceId, "price_monthly", StringComparison.OrdinalIgnoreCase))
            return _options.ProPriceIdMonthly;

        if (string.Equals(priceId, "price_yearly", StringComparison.OrdinalIgnoreCase))
            return _options.ProPriceIdYearly;

        return priceId;
    }

    /// <summary>
    /// Stripe sends current_period_end as 0 when a subscription is canceled/deleted;
    /// the SDK converts that to Unix epoch (1970-01-01 UTC). Treat that as no period end.
    /// </summary>
    internal static DateTime? NormalizePeriodEnd(DateTime? value)
    {
        if (!value.HasValue)
            return null;
        var utc =
            value.Value.Kind == DateTimeKind.Utc
                ? value.Value
                : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        return utc.Year < 1980 ? null : utc;
    }

    /// <summary>
    /// When the subscription root has no current_period_end (e.g. some webhook payloads),
    /// read it from the first subscription item in the raw JSON.
    /// </summary>
    internal static DateTime? GetCurrentPeriodEndFromFirstItemInPayload(string jsonPayload)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonPayload);
            var root = doc.RootElement;
            if (!root.TryGetProperty("data", out var data))
                return null;
            if (!data.TryGetProperty("object", out var obj))
                return null;
            if (!obj.TryGetProperty("items", out var items))
                return null;
            if (!items.TryGetProperty("data", out var dataArray) || dataArray.GetArrayLength() == 0)
                return null;
            var firstItem = dataArray[0];
            if (!firstItem.TryGetProperty("current_period_end", out var periodEndEl))
                return null;
            if (periodEndEl.ValueKind != JsonValueKind.Number)
                return null;
            long unixSeconds;
            try
            {
                unixSeconds = periodEndEl.GetInt64();
            }
            catch
            {
                return null;
            }
            if (unixSeconds <= 0)
                return null;
            var utc = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime;
            return NormalizePeriodEnd(utc);
        }
        catch
        {
            return null;
        }
    }

    public (StripeWebhookPayload? Payload, string? Error) TryParseWebhookEvent(
        string jsonPayload,
        string signatureHeader
    )
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                jsonPayload,
                signatureHeader,
                _options.WebhookSecret,
                throwOnApiVersionMismatch: false
            );

            string? clientReferenceId = null;
            string? customerId = null;
            string? subscriptionId = null;
            DateTime? currentPeriodEndUtc = null;
            string? subscriptionStatus = null;
            var cancelAtPeriodEnd = false;

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    var session = stripeEvent.Data.Object as Session;
                    if (session != null)
                    {
                        clientReferenceId = session.ClientReferenceId;
                        customerId = session.CustomerId;
                        subscriptionId = session.SubscriptionId;
                        if (session.SubscriptionId != null)
                        {
                            var subService = new SubscriptionService();
                            var sub = subService.Get(session.SubscriptionId);
                            currentPeriodEndUtc = NormalizePeriodEnd(sub.CurrentPeriodEnd);
                            subscriptionStatus = sub.Status;
                            cancelAtPeriodEnd = sub.CancelAtPeriodEnd;
                        }
                    }
                    break;
                case "customer.subscription.updated":
                case "customer.subscription.deleted":
                    var subscription = stripeEvent.Data.Object as Subscription;
                    if (subscription != null)
                    {
                        customerId = subscription.CustomerId;
                        subscriptionId = subscription.Id;
                        var periodEnd = GetCurrentPeriodEndFromFirstItemInPayload(jsonPayload);
                        currentPeriodEndUtc = NormalizePeriodEnd(periodEnd);
                        subscriptionStatus = subscription.Status;
                        cancelAtPeriodEnd = subscription.CancelAtPeriodEnd;
                    }
                    break;
            }

            var payload = new StripeWebhookPayload(
                stripeEvent.Id,
                stripeEvent.Type,
                clientReferenceId,
                customerId,
                subscriptionId,
                currentPeriodEndUtc,
                subscriptionStatus,
                cancelAtPeriodEnd
            );

            return (payload, null);
        }
        catch (StripeException ex)
        {
            return (null, ex.Message);
        }
    }

    public Task<bool> HasProcessedEventAsync(
        string eventId,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(ProcessedEvents.ContainsKey(eventId));
    }

    public Task MarkEventProcessedAsync(
        string eventId,
        CancellationToken cancellationToken = default
    )
    {
        ProcessedEvents.TryAdd(eventId, 0);
        return Task.CompletedTask;
    }

    public async Task<string> CreateBillingPortalSessionAsync(
        string customerId,
        string returnUrl,
        CancellationToken cancellationToken = default
    )
    {
        var portalService = new Stripe.BillingPortal.SessionService();
        var session = await portalService.CreateAsync(
            new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = customerId,
                ReturnUrl = returnUrl,
            },
            cancellationToken: cancellationToken
        );
        return session.Url;
    }
}
