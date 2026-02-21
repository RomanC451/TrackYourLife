using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Contracts.Integration.Users;

/// <summary>
/// Request to get user data needed for billing (checkout, portal) by user id.
/// Used by Payments module without depending on Users module.
/// </summary>
public sealed record GetUserForBillingByIdRequest(UserId UserId);

public sealed record UserForBillingDto(
    UserId UserId,
    string Email,
    string? StripeCustomerId,
    bool HasActiveProSubscription
);

public sealed record GetUserForBillingByIdResponse(UserForBillingDto? Data, List<Error> Errors);

/// <summary>
/// Request to get user data by Stripe customer id (e.g. for webhooks).
/// Used by Payments module without depending on Users module.
/// </summary>
public sealed record GetUserForBillingByStripeCustomerIdRequest(string StripeCustomerId);

public sealed record UserForBillingByStripeCustomerIdDto(UserId UserId, string Email);

public sealed record GetUserForBillingByStripeCustomerIdResponse(
    UserForBillingByStripeCustomerIdDto? Data,
    List<Error> Errors
);
