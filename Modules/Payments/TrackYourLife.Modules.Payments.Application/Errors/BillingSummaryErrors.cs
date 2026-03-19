using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Payments.Application.Errors;

public static class BillingSummaryErrors
{
    public static readonly Error UserNotFound = new(
        "BillingSummary.UserNotFound",
        "User not found.",
        404
    );

    public static readonly Error NoStripeCustomer = new(
        "BillingSummary.NoStripeCustomer",
        "No Stripe customer linked to this account.",
        404
    );
}

