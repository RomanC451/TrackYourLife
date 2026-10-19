using System.Net;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

namespace TrackYourLife.Modules.GuitarTabs.Application.UnitTests.Features.GuitarTabSearch;

public sealed class UltimateGuitarTabPageParserTests
{
    [Fact]
    public void Parse_ReadsWikiContentAndTheFirstChordShape()
    {
        const string payload = """
            {
              "store": {
                "page": {
                  "data": {
                    "tab": {
                      "id": 5059138,
                      "song_name": "Viu",
                      "artist_name": "Bosquito",
                      "type": "Chords",
                      "tonality_name": "F#",
                      "rating": 4.55,
                      "votes": 5,
                      "tab_url": "https://tabs.ultimate-guitar.com/tab/bosquito/viu-chords-5059138",
                      "recording": {
                        "video_urls": [
                          "https://youtu.be/RBumgq5yVrA?t=12",
                          "https://example.com/not-youtube"
                        ]
                      }
                    },
                    "tab_view": {
                      "blocked": false,
                      "meta": {
                        "tonality": "F#m",
                        "capo": 2,
                        "tuning": { "name": "Standard", "value": "E A D G B E", "index": 1 }
                      },
                      "wiki_tab": {
                        "content": "[Intro]\n[ch]Am[/ch]\n[Verse 1]\n[tab][ch]G[/ch]\nline[/tab]"
                      },
                      "applicature": {
                        "Am": [
                          {
                            "frets": [0, 1, 2, 2, 0, -1],
                            "fret": 0,
                            "listCapos": []
                          }
                        ]
                      }
                    }
                  }
                }
              }
            }
            """;

        var html = $"""
            <html><body><div class="js-store" data-content="{WebUtility.HtmlEncode(payload)}"></div></body></html>
            """;

        var result = UltimateGuitarTabPageParser.Parse(html);

        result.IsSuccess.Should().BeTrue();
        result.Value.SongName.Should().Be("Viu");
        result.Value.ArtistName.Should().Be("Bosquito");
        result.Value.Type.Should().Be("Chords");
        result.Value.Tonality.Should().Be("F#m");
        result.Value.TuningName.Should().Be("Standard");
        result.Value.Tuning.Should().Be("E A D G B E");
        result.Value.Capo.Should().Be(2);
        result.Value.Content.Should().Contain("[ch]Am[/ch]");
        result.Value.Chords.Should().ContainSingle();
        result.Value.Chords[0].Name.Should().Be("Am");
        result.Value.Chords[0].Frets.Should().Equal(0, 1, 2, 2, 0, -1);
        result.Value.YoutubeUrl.Should().Be("https://www.youtube.com/watch?v=RBumgq5yVrA");
    }

    [Fact]
    public void Parse_WhenTheTabIsBlocked_ReturnsContentUnavailable()
    {
        const string payload = """
            {
              "store": {
                "page": {
                  "data": {
                    "tab": {
                      "id": 1,
                      "song_name": "Song",
                      "artist_name": "Artist",
                      "type": "Official",
                      "tab_url": "https://tabs.ultimate-guitar.com/tab/artist/song-official-1"
                    },
                    "tab_view": {
                      "blocked": true,
                      "wiki_tab": { "content": "[ch]Am[/ch]" }
                    }
                  }
                }
              }
            }
            """;

        var html = $"""
            <html><body><div class="js-store" data-content="{WebUtility.HtmlEncode(payload)}"></div></body></html>
            """;

        var result = UltimateGuitarTabPageParser.Parse(html);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GuitarTabSearchErrors.ContentUnavailable);
    }
}
