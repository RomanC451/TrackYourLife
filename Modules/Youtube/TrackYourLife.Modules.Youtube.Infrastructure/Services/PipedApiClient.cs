using System.Text.Json;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Youtube.Infrastructure.Options;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Services;

internal sealed class PipedApiClient(HttpClient httpClient, IOptions<YoutubeApiOptions> options)
    : IPipedApiClient
{
    private readonly YoutubeApiOptions _options = options.Value;

    public async Task<PipedPlaybackInfo> GetPlaybackInfoAsync(
        string videoId,
        CancellationToken cancellationToken = default
    )
    {
        var embedUrl = BuildEmbedUrl(videoId);

        try
        {
            using var response = await httpClient.GetAsync($"streams/{videoId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new PipedPlaybackInfo(null, embedUrl);
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync(
                cancellationToken
            );
            using var document = await JsonDocument.ParseAsync(
                responseStream,
                cancellationToken: cancellationToken
            );

            var directPlaybackUrl = SelectBestPlaybackUrl(document.RootElement);
            return new PipedPlaybackInfo(directPlaybackUrl, embedUrl);
        }
        catch
        {
            return new PipedPlaybackInfo(null, embedUrl);
        }
    }

    private string BuildEmbedUrl(string videoId)
    {
        var embedUrl = new UriBuilder(_options.PipedFrontendBaseUrl.TrimEnd('/'))
        {
            Path = $"/embed/{Uri.EscapeDataString(videoId)}",
            Query = "autoplay=true&quality=best",
        };

        return embedUrl.Uri.ToString();
    }

    private string? SelectBestPlaybackUrl(JsonElement root)
    {
        var hls = ReadString(root, "hls");
        if (!string.IsNullOrWhiteSpace(hls))
        {
            return ToAbsolutePlaybackUrl(hls);
        }

        if (!root.TryGetProperty("videoStreams", out var videoStreams))
        {
            return null;
        }

        if (videoStreams.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        string? bestProgressiveUrl = null;
        long bestProgressiveBitrate = -1;
        string? bestAnyUrl = null;
        long bestAnyBitrate = -1;

        foreach (var stream in videoStreams.EnumerateArray())
        {
            var url = ReadString(stream, "url");
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var bitrate = ReadLong(stream, "bitrate");
            var isVideoOnly = ReadBool(stream, "videoOnly");

            if (!isVideoOnly && bitrate > bestProgressiveBitrate)
            {
                bestProgressiveBitrate = bitrate;
                bestProgressiveUrl = url;
            }

            if (bitrate > bestAnyBitrate)
            {
                bestAnyBitrate = bitrate;
                bestAnyUrl = url;
            }
        }

        return ToAbsolutePlaybackUrl(bestProgressiveUrl ?? bestAnyUrl);
    }

    private string? ToAbsolutePlaybackUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            return url;
        }

        if (
            !Uri.TryCreate(_options.PipedProxyBaseUrl, UriKind.Absolute, out var proxyBase)
            || !Uri.TryCreate(proxyBase, url, out var absoluteUrl)
        )
        {
            return null;
        }

        return absoluteUrl.ToString();
    }

    private static string? ReadString(JsonElement element, string propertyName)
    {
        return
            element.TryGetProperty(propertyName, out var property)
            && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static long ReadLong(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return 0;
        }

        return property.ValueKind == JsonValueKind.Number && property.TryGetInt64(out var value)
            ? value
            : 0;
    }

    private static bool ReadBool(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return false;
        }

        return property.ValueKind == JsonValueKind.True;
    }
}
