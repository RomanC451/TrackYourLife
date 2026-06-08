namespace TrackYourLife.App.E2e;

internal static class E2EMocks
{
    public const string ConfigurationKey = "FeatureFlags:UseE2eMocks";

    public static bool IsEnabled(IConfiguration configuration) =>
        configuration.GetValue<bool>(ConfigurationKey);
}
