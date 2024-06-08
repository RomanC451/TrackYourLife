namespace TrackYourLife.Common.Infrastructure.Options;

public sealed class SupaBaseOptions
{
    public const string ConfigurationSection = "SupaBase";

    public string Url { get; init; } = string.Empty;

    public string Key { get; init; } = string.Empty;
}
