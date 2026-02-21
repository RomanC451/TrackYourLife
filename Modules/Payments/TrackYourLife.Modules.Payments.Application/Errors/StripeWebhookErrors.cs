using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Payments.Application.Errors;

public static class StripeWebhookErrors
{
    public static Error ParseFailed(string message) => new("StripeWebhook.ParseFailed", message);

    public static readonly Error PayloadNull = new(
        "StripeWebhook.PayloadNull",
        "Failed to parse webhook event."
    );

    public static class CheckoutSessionCompleted
    {
        public static readonly Error MissingPayload = new(
            "StripeWebhook.CheckoutSessionCompleted.MissingPayload",
            "Webhook payload is required."
        );

        public static readonly Error InvalidClientReferenceId = new(
            "StripeWebhook.CheckoutSessionCompleted.InvalidClientReferenceId",
            "Missing or invalid client_reference_id in webhook payload."
        );

        public static readonly Error MissingCustomerId = new(
            "StripeWebhook.CheckoutSessionCompleted.MissingCustomerId",
            "Missing customer ID in webhook payload."
        );

        public static readonly Error MissingCurrentPeriodEnd = new(
            "StripeWebhook.CheckoutSessionCompleted.MissingCurrentPeriodEnd",
            "Missing subscription period end in webhook payload."
        );

        public static readonly Error UpgradeFailed = new(
            "StripeWebhook.CheckoutSessionCompleted.UpgradeFailed",
            "Failed to upgrade user to Pro."
        );
    }

    public static class SubscriptionUpdated
    {
        public static readonly Error MissingCustomerId = new(
            "StripeWebhook.SubscriptionUpdated.MissingCustomerId",
            "Missing customer ID in subscription updated webhook payload."
        );

        public static readonly Error InvalidSubscriptionStatus = new(
            "StripeWebhook.SubscriptionUpdated.InvalidSubscriptionStatus",
            "Missing or invalid subscription status in webhook payload."
        );

        public static readonly Error UserNotFound = new(
            "StripeWebhook.SubscriptionUpdated.UserNotFound",
            "User not found for Stripe customer.",
            404
        );

        public static readonly Error UpdatePeriodEndFailed = new(
            "StripeWebhook.SubscriptionUpdated.UpdatePeriodEndFailed",
            "Failed to update subscription period end."
        );

        public static readonly Error DowngradeFailed = new(
            "StripeWebhook.SubscriptionUpdated.DowngradeFailed",
            "Failed to downgrade subscription."
        );
    }

    public static class SubscriptionDeleted
    {
        public static readonly Error MissingCustomerId = new(
            "StripeWebhook.SubscriptionDeleted.MissingCustomerId",
            "Missing customer ID in subscription deleted webhook payload."
        );

        public static readonly Error InvalidSubscriptionStatus = new(
            "StripeWebhook.SubscriptionDeleted.InvalidSubscriptionStatus",
            "Missing or invalid subscription status in webhook payload."
        );

        public static readonly Error UserNotFound = new(
            "StripeWebhook.SubscriptionDeleted.UserNotFound",
            "User not found for Stripe customer.",
            404
        );

        public static readonly Error DowngradeFailed = new(
            "StripeWebhook.SubscriptionDeleted.DowngradeFailed",
            "Failed to downgrade subscription."
        );
    }
}
