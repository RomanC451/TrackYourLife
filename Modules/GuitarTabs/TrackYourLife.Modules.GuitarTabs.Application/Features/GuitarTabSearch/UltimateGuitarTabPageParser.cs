using System.Text.Json;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

public static class UltimateGuitarTabPageParser
{
    public static Result<GuitarTabContentResponse> Parse(string html)
    {
        return UltimateGuitarPageStore.Read(html, ReadTab);
    }

    private static Result<GuitarTabContentResponse> ReadTab(JsonElement data)
    {
        if (
            !data.TryGetProperty("tab", out var tab)
            || tab.ValueKind != JsonValueKind.Object
            || !data.TryGetProperty("tab_view", out var tabView)
            || tabView.ValueKind != JsonValueKind.Object
        )
        {
            return Result.Failure<GuitarTabContentResponse>(GuitarTabSearchErrors.InvalidResponse);
        }

        var content = ReadContent(tabView);
        var blocked = ReadBool(tabView, "blocked");
        if (blocked || string.IsNullOrWhiteSpace(content))
        {
            return Result.Failure<GuitarTabContentResponse>(
                GuitarTabSearchErrors.ContentUnavailable
            );
        }

        var songName = ReadString(tab, "song_name");
        var artistName = ReadString(tab, "artist_name");
        var tabUrl = ReadString(tab, "tab_url");
        if (
            string.IsNullOrWhiteSpace(songName)
            || string.IsNullOrWhiteSpace(artistName)
            || string.IsNullOrWhiteSpace(tabUrl)
        )
        {
            return Result.Failure<GuitarTabContentResponse>(GuitarTabSearchErrors.InvalidResponse);
        }

        var (key, tuningName, tuning, capo) = ReadPerformance(tab, tabView);

        return Result.Success(
            new GuitarTabContentResponse(
                ReadInt64(tab, "id") ?? 0,
                songName,
                artistName,
                ReadString(tab, "type") ?? "Tab",
                key,
                tuningName,
                tuning,
                capo,
                ReadDouble(tab, "rating"),
                ReadInt(tab, "votes") ?? 0,
                content.Replace("\r\n", "\n"),
                tabUrl,
                ReadChords(tabView),
                ReadYoutubeUrl(tab, tabView)
            )
        );
    }

    private static (string? Key, string? TuningName, string? Tuning, int? Capo) ReadPerformance(
        JsonElement tab,
        JsonElement tabView
    )
    {
        string? key = null;
        string? tuningName = null;
        string? tuning = null;
        int? capo = null;

        if (
            tabView.TryGetProperty("meta", out var meta)
            && meta.ValueKind == JsonValueKind.Object
        )
        {
            key = NullIfEmpty(ReadString(meta, "tonality"));
            if (
                meta.TryGetProperty("tuning", out var tuningElement)
                && tuningElement.ValueKind == JsonValueKind.Object
            )
            {
                tuningName = NullIfEmpty(ReadString(tuningElement, "name"));
                tuning = NullIfEmpty(ReadString(tuningElement, "value"));
            }

            var capoFret = ReadInt(meta, "capo");
            if (capoFret is > 0)
            {
                capo = capoFret;
            }
        }

        key ??= NullIfEmpty(ReadString(tab, "tonality_name"));
        return (key, tuningName, tuning, capo);
    }

    private static string? ReadYoutubeUrl(JsonElement tab, JsonElement tabView)
    {
        if (
            tab.TryGetProperty("recording", out var recording)
            && recording.ValueKind == JsonValueKind.Object
            && recording.TryGetProperty("video_urls", out var videoUrls)
            && videoUrls.ValueKind == JsonValueKind.Array
        )
        {
            foreach (var videoUrl in videoUrls.EnumerateArray())
            {
                var candidate = videoUrl.ValueKind switch
                {
                    JsonValueKind.String => videoUrl.GetString(),
                    JsonValueKind.Object => ReadString(videoUrl, "url") ?? ReadString(videoUrl, "video_url"),
                    _ => null,
                };
                if (YoutubeVideoUrl.TryNormalize(candidate, out var normalized))
                {
                    return normalized;
                }
            }
        }

        if (
            tabView.TryGetProperty("lyrics_lines", out var lyrics)
            && lyrics.ValueKind == JsonValueKind.Object
            && YoutubeVideoUrl.TryNormalize(
                "https://www.youtube.com/watch?v=" + ReadString(lyrics, "youtube_id"),
                out var lyricsVideo
            )
        )
        {
            return lyricsVideo;
        }

        return null;
    }

    private static string? ReadContent(JsonElement tabView)
    {
        if (
            !tabView.TryGetProperty("wiki_tab", out var wikiTab)
            || wikiTab.ValueKind != JsonValueKind.Object
        )
        {
            return null;
        }

        return ReadString(wikiTab, "content");
    }

    private static List<GuitarTabChordResponse> ReadChords(JsonElement tabView)
    {
        var chords = new List<GuitarTabChordResponse>();
        if (
            !tabView.TryGetProperty("applicature", out var applicature)
            || applicature.ValueKind != JsonValueKind.Object
        )
        {
            return chords;
        }

        foreach (var chord in applicature.EnumerateObject())
        {
            if (chord.Value.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            var shapes = chord.Value.EnumerateArray();
            if (!shapes.MoveNext())
            {
                continue;
            }

            var mapped = MapChord(chord.Name, shapes.Current);
            if (mapped is not null)
            {
                chords.Add(mapped);
            }
        }

        return chords;
    }

    private static GuitarTabChordResponse? MapChord(string name, JsonElement shape)
    {
        if (
            string.IsNullOrWhiteSpace(name)
            || !shape.TryGetProperty("frets", out var fretsElement)
            || fretsElement.ValueKind != JsonValueKind.Array
            || fretsElement.GetArrayLength() != 6
        )
        {
            return null;
        }

        var frets = new int[6];
        var index = 0;
        foreach (var fret in fretsElement.EnumerateArray())
        {
            frets[index++] = fret.ValueKind == JsonValueKind.Number && fret.TryGetInt32(out var value)
                ? value
                : -1;
        }

        int? barreFret = null;
        int? barreFromString = null;
        int? barreToString = null;
        if (
            shape.TryGetProperty("listCapos", out var capos)
            && capos.ValueKind == JsonValueKind.Array
        )
        {
            var capoItems = capos.EnumerateArray();
            if (capoItems.MoveNext())
            {
                var capo = capoItems.Current;
                barreFret = ReadInt(capo, "fret");
                barreFromString = ReadInt(capo, "startString");
                barreToString = ReadInt(capo, "lastString");
            }
        }

        return new GuitarTabChordResponse(
            name,
            frets,
            ReadInt(shape, "fret") ?? 0,
            barreFret,
            barreFromString,
            barreToString
        );
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

    private static bool ReadBool(JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var property)
            && (
                (property.ValueKind == JsonValueKind.True)
                || (property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var value) && value != 0)
            );
    }

    private static int? ReadInt(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property) || property.ValueKind != JsonValueKind.Number)
        {
            return null;
        }

        return property.TryGetInt32(out var value) ? value : null;
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
