using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Payments.Application.Errors;

public static class BillingPortalErrors
{
    public static readonly Error UserNotFound = new(
        "BillingPortal.UserNotFound",
        "User not found.",
        404
    );

    public static readonly Error NoStripeCustomer = new(
        "BillingPortal.NoStripeCustomer",
        "No Stripe customer linked to this account.",
        404
    );
}
