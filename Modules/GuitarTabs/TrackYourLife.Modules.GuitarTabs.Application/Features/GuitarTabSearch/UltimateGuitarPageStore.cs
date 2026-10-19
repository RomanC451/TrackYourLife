using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

public static partial class UltimateGuitarPageStore
{
    public static Result<T> Read<T>(string html, Func<JsonElement, Result<T>> read)
    {
        var match = StoreContentRegex().Match(html);
        if (!match.Success)
        {
            return Result.Failure<T>(GuitarTabSearchErrors.InvalidResponse);
        }

        var encoded = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;

        try
        {
            using var document = JsonDocument.Parse(WebUtility.HtmlDecode(encoded));
            if (
                !document.RootElement.TryGetProperty("store", out var store)
                || !store.TryGetProperty("page", out var pageElement)
                || !pageElement.TryGetProperty("data", out var data)
            )
            {
                return Result.Failure<T>(GuitarTabSearchErrors.InvalidResponse);
            }

            return read(data);
        }
        catch (JsonException)
        {
            return Result.Failure<T>(GuitarTabSearchErrors.InvalidResponse);
        }
    }

    [GeneratedRegex(
        "<div[^>]*class=\"[^\"]*\\bjs-store\\b[^\"]*\"[^>]*data-content=\"([^\"]*)\"|<div[^>]*data-content=\"([^\"]*)\"[^>]*class=\"[^\"]*\\bjs-store\\b[^\"]*\"",
        RegexOptions.IgnoreCase
    )]
    private static partial Regex StoreContentRegex();
}
