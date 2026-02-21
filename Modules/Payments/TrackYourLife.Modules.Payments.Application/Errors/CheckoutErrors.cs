using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Payments.Application.Errors;

public static class CheckoutErrors
{
    public static readonly Error UserNotFound = new(
        "Checkout.UserNotFound",
        "User not found.",
        404
    );

    public static readonly Error AlreadySubscribedPro = new(
        "Checkout.AlreadySubscribed",
        "You already have an active Pro subscription.",
        400
    );

    public static readonly Error AlreadySubscribed = new(
        "Checkout.AlreadySubscribed",
        "You already have an active subscription for this plan.",
        400
    );
}
