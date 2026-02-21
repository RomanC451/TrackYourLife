namespace TrackYourLife.Modules.Payments.Infrastructure.Options;

public sealed class StripeOptions
{
    public const string ConfigurationSection = "Stripe";

    public string SecretKey { get; set; } = string.Empty;

    public string WebhookSecret { get; set; } = string.Empty;

    public string ProPriceIdMonthly { get; set; } = string.Empty;

    public string ProPriceIdYearly { get; set; } = string.Empty;
}
