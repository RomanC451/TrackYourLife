using System.Text.Json;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

public static class UltimateGuitarSearchPageParser
{
    public static Result<GuitarTabSearchResponse> Parse(string html, string query)
    {
        return UltimateGuitarPageStore.Read(html, data =>
        {
            var results = new List<GuitarTabSearchResultResponse>();
            if (
                data.TryGetProperty("results", out var resultsElement)
                && resultsElement.ValueKind == JsonValueKind.Array
            )
            {
                foreach (var result in resultsElement.EnumerateArray())
                {
                    var mapped = MapResult(result);
                    if (mapped is not null)
                    {
                        results.Add(mapped);
                    }
                }
            }

            var (page, totalPages) = ReadPagination(data);
            var resultsCount = ReadInt(data, "results_count") ?? results.Count;

            return Result.Success(
                new GuitarTabSearchResponse(query, resultsCount, page, totalPages, results)
            );
        });
    }

    private static GuitarTabSearchResultResponse? MapResult(JsonElement result)
    {
        var songName = ReadString(result, "song_name");
        var artistName = ReadString(result, "artist_name");
        var tabUrl = ReadString(result, "tab_url");

        if (
            string.IsNullOrWhiteSpace(songName)
            || string.IsNullOrWhiteSpace(artistName)
            || string.IsNullOrWhiteSpace(tabUrl)
        )
        {
            return null;
        }

        return new GuitarTabSearchResultResponse(
            ReadInt64(result, "id"),
            songName,
            artistName,
            ResolveType(ReadString(result, "type"), ReadString(result, "marketing_type")),
            ReadInt(result, "votes") ?? 0,
            ReadDouble(result, "rating"),
            NullIfEmpty(ReadString(result, "tonality_name")),
            ReadInt(result, "version") ?? 0,
            ReadString(result, "part") ?? string.Empty,
            tabUrl,
            NullIfEmpty(ReadString(result, "artist_url"))
        );
    }

    private static (int Page, int TotalPages) ReadPagination(JsonElement data)
    {
        if (
            !data.TryGetProperty("pagination", out var pagination)
            || pagination.ValueKind != JsonValueKind.Object
        )
        {
            return (1, 1);
        }

        var page = Math.Max(1, ReadInt(pagination, "current") ?? 1);
        var totalPages = Math.Max(1, ReadInt(pagination, "total") ?? 1);
        return (page, totalPages);
    }

    private static string ResolveType(string? type, string? marketingType)
    {
        if (!string.IsNullOrWhiteSpace(type))
        {
            return type;
        }

        if (string.IsNullOrWhiteSpace(marketingType))
        {
            return "Tab";
        }

        if (marketingType.Equals("official", StringComparison.OrdinalIgnoreCase))
        {
            return "Official";
        }

        if (marketingType.Equals("TabPro", StringComparison.OrdinalIgnoreCase))
        {
            return "Pro";
        }

        return marketingType;
    }

    private static string? ReadString(JsonElement element, string name)
    {
        if (
            !element.TryGetProperty(name, out var property)
            || property.ValueKind != JsonValueKind.String
        )
        {
            return null;
        }

        return property.GetString();
    }

    private static int? ReadInt(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property) || property.ValueKind != JsonValueKind.Number)
        {
            return null;
        }

        if (property.TryGetInt32(out var value))
        {
            return value;
        }

        return property.TryGetInt64(out var longValue) && longValue <= int.MaxValue
            ? (int)longValue
            : null;
    }

    private static long? ReadInt64(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property) || property.ValueKind != JsonValueKind.Number)
        {
            return null;
        }

        return property.TryGetInt64(out var value) ? value : null;
    }

    private static double? ReadDouble(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property) || property.ValueKind != JsonValueKind.Number)
        {
            return null;
        }

        return property.TryGetDouble(out var value) ? value : null;
    }

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
