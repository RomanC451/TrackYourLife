using System.Xml;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

public static class YoutubeDuration
{
    public static int? ToSeconds(string? isoDuration)
    {
        if (string.IsNullOrEmpty(isoDuration))
        {
            return null;
        }

        try
        {
            var seconds = (int)Math.Round(XmlConvert.ToTimeSpan(isoDuration).TotalSeconds);
            return seconds > 0 ? seconds : null;
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
