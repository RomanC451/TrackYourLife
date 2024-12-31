using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Common.Infrastructure.Options;

public sealed class SupaBaseOptions : IOptions
{
    public const string ConfigurationSection = "SupaBase";

    public string Url { get; init; } = string.Empty;

    public string Key { get; init; } = string.Empty;
}
