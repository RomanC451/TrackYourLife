namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Options;

public sealed class UltimateGuitarOptions
{
    public const string ConfigurationSection = "UltimateGuitar";

    public string SearchBaseUrl { get; set; } = string.Empty;
}
