namespace TrackYourLife.Modules.Youtube.Application.Options;

public sealed class YoutubeModuleOptions
{
    public const string ConfigurationSection = "Youtube";

    public int MaxCategoriesForPro { get; set; } = 10;
}
