using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Features.Users;

public sealed record UserReadModel(
    UserId Id,
    string FirstName,
    string LastName,
    string Email,
    string PasswordHash,
    DateTime? VerifiedOnUtc,
    PlanType PlanType,
    string? StripeCustomerId,
    DateTime? SubscriptionEndsAtUtc,
    SubscriptionStatus? SubscriptionStatus,
    bool SubscriptionCancelAtPeriodEnd = false
) : IReadModel<UserId>;
