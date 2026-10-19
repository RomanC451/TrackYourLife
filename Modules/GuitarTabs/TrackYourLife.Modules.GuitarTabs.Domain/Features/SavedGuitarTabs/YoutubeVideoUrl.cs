using System.Text.RegularExpressions;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

public static partial class YoutubeVideoUrl
{
    private static readonly Regex VideoIdPattern = VideoIdRegex();

    public static bool TryNormalize(string? input, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var match = VideoIdPattern.Match(input.Trim());
        if (!match.Success)
        {
            return false;
        }

        normalized = $"https://www.youtube.com/watch?v={match.Groups[1].Value}";
        return normalized.Length <= SavedGuitarTab.YoutubeUrlMaxLength;
    }

    [GeneratedRegex(
        @"(?:youtube\.com/(?:watch\?(?:[^#]*&)?v=|embed/|shorts/|live/)|youtu\.be/)([A-Za-z0-9_-]{11})",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
    )]
    private static partial Regex VideoIdRegex();
}
