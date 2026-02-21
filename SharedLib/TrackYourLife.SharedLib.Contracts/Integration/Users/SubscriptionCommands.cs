using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Contracts.Integration.Users;

/// <summary>
/// Request to upgrade a user to Pro (from Stripe checkout or webhook).
/// Consumers in Users module apply the change.
/// </summary>
public sealed record UpgradeToProRequest(
    UserId UserId,
    string StripeCustomerId,
    DateTime PeriodEndUtc,
    bool CancelAtPeriodEnd = false
);

public sealed record UpgradeToProResponse(List<Error> Errors);

/// <summary>
/// Request to downgrade a user from Pro. Subscription status from Stripe is stored on the user.
/// </summary>
public sealed record DowngradeProRequest(UserId UserId, SubscriptionStatus SubscriptionStatus);

public sealed record DowngradeProResponse(List<Error> Errors);

/// <summary>
/// Request to update the Pro subscription period end date, status, and cancel-at-period-end flag.
/// </summary>
public sealed record UpdateProSubscriptionPeriodEndRequest(
    UserId UserId,
    DateTime PeriodEndUtc,
    SubscriptionStatus SubscriptionStatus,
    bool CancelAtPeriodEnd = false
);

public sealed record UpdateProSubscriptionPeriodEndResponse(List<Error> Errors);
