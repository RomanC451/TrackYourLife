namespace TrackYourLife.Modules.Youtube.Infrastructure.Options;

public sealed class YoutubeApiOptions
{
    public const string ConfigurationSection = "YoutubeApi";

    public string ApiKey { get; set; } = string.Empty;

    public TimeSpan SearchCacheDuration { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan ChannelVideosCacheDuration { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan VideoDetailsCacheDuration { get; set; } = TimeSpan.FromHours(2);
}
