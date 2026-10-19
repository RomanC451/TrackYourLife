using System.Net;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

namespace TrackYourLife.Modules.GuitarTabs.Application.UnitTests.Features.GuitarTabSearch;

public sealed class UltimateGuitarSearchPageParserTests
{
    [Fact]
    public void Parse_ReadsTitleSearchResultsFromThePageStore()
    {
        const string payload = """
            {
              "store": {
                "page": {
                  "data": {
                    "results_count": 15,
                    "pagination": { "total": 2, "current": 1 },
                    "results": [
                      {
                        "id": 5059138,
                        "song_name": "Viu",
                        "artist_name": "Bosquito",
                        "type": "Chords",
                        "votes": 5,
                        "rating": 4.55,
                        "tonality_name": "C",
                        "version": 2,
                        "part": "Intro",
                        "tab_url": "https://tabs.ultimate-guitar.com/tab/bosquito/viu-chords-5059138",
                        "artist_url": "https://www.ultimate-guitar.com/artist/bosquito_123239"
                      },
                      {
                        "song_name": "Wonderwall",
                        "artist_name": "Oasis",
                        "marketing_type": "official",
                        "tab_url": "https://www.ultimate-guitar.com/pro/?song=wonderwall"
                      },
                      {
                        "song_name": "Skipped"
                      }
                    ]
                  }
                }
              }
            }
            """;

        var html = $"""
            <html><body><div class="js-store" data-content="{WebUtility.HtmlEncode(payload)}"></div></body></html>
            """;

        var result = UltimateGuitarSearchPageParser.Parse(html, "viu");

        result.IsSuccess.Should().BeTrue();
        result.Value.Query.Should().Be("viu");
        result.Value.ResultsCount.Should().Be(15);
        result.Value.Page.Should().Be(1);
        result.Value.TotalPages.Should().Be(2);
        result.Value.Results.Should().HaveCount(2);

        var first = result.Value.Results[0];
        first.Id.Should().Be(5059138);
        first.SongName.Should().Be("Viu");
        first.ArtistName.Should().Be("Bosquito");
        first.Type.Should().Be("Chords");
        first.Votes.Should().Be(5);
        first.Rating.Should().BeApproximately(4.55, 0.001);
        first.Tonality.Should().Be("C");
        first.Version.Should().Be(2);
        first.Part.Should().Be("Intro");

        result.Value.Results[1].Type.Should().Be("Official");
        result.Value.Results[1].Id.Should().BeNull();
    }

    [Fact]
    public void Parse_WhenStoreIsMissing_ReturnsInvalidResponse()
    {
        var result = UltimateGuitarSearchPageParser.Parse("<html></html>", "viu");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GuitarTabSearchErrors.InvalidResponse);
    }
}
