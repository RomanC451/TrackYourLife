namespace TrackYourLife.SharedLib.Contracts;

/// <summary>
/// Stripe subscription status. See https://docs.stripe.com/api/subscriptions/object#subscription_object-status
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>The subscription is currently in a trial period.</summary>
    Trialing = 0,

    /// <summary>The subscription is in good standing.</summary>
    Active = 1,

    /// <summary>The customer must make a successful payment within 23 hours to activate the subscription.</summary>
    Incomplete = 2,

    /// <summary>The initial payment failed and the customer didn't pay within 23 hours of creation.</summary>
    IncompleteExpired = 3,

    /// <summary>Payment on the latest finalized invoice either failed or wasn't attempted.</summary>
    PastDue = 4,

    /// <summary>The subscription was canceled. Terminal state.</summary>
    Canceled = 5,

    /// <summary>The latest invoice hasn't been paid but the subscription remains in place.</summary>
    Unpaid = 6,

    /// <summary>The subscription has ended its trial without a default payment method and is paused.</summary>
    Paused = 7
}

/// <summary>
/// Converts between <see cref="SubscriptionStatus"/> and Stripe's API string values.
/// </summary>
public static class SubscriptionStatusMapping
{
    private static readonly Dictionary<SubscriptionStatus, string> ToStripe = new()
    {
        [SubscriptionStatus.Trialing] = "trialing",
        [SubscriptionStatus.Active] = "active",
        [SubscriptionStatus.Incomplete] = "incomplete",
        [SubscriptionStatus.IncompleteExpired] = "incomplete_expired",
        [SubscriptionStatus.PastDue] = "past_due",
        [SubscriptionStatus.Canceled] = "canceled",
        [SubscriptionStatus.Unpaid] = "unpaid",
        [SubscriptionStatus.Paused] = "paused"
    };

    private static readonly Dictionary<string, SubscriptionStatus> FromStripe = new(StringComparer.OrdinalIgnoreCase)
    {
        ["trialing"] = SubscriptionStatus.Trialing,
        ["active"] = SubscriptionStatus.Active,
        ["incomplete"] = SubscriptionStatus.Incomplete,
        ["incomplete_expired"] = SubscriptionStatus.IncompleteExpired,
        ["past_due"] = SubscriptionStatus.PastDue,
        ["canceled"] = SubscriptionStatus.Canceled,
        ["unpaid"] = SubscriptionStatus.Unpaid,
        ["paused"] = SubscriptionStatus.Paused
    };

    public static string ToStripeString(this SubscriptionStatus status) => ToStripe[status];

    public static SubscriptionStatus? Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return FromStripe.TryGetValue(value.Trim(), out var status) ? status : null;
    }
}
