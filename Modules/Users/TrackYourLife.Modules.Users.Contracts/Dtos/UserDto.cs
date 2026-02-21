using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Contracts.Dtos;

public sealed record UserDto(
    UserId Id,
    string Email,
    string FirstName,
    string LastName,
    PlanType PlanType,
    DateTime? SubscriptionEndsAtUtc,
    SubscriptionStatus? SubscriptionStatus,
    bool SubscriptionCancelAtPeriodEnd = false
);
