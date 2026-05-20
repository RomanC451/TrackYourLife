using System.Net;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyCategoryWatchCounters.Models;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Features;

[Collection("Youtube Integration Tests")]
public class DailyCategoryWatchCountersTests(YoutubeFunctionalTestWebAppFactory factory)
    : YoutubeBaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetDailyCategoryWatchCounters_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/settings/daily-category-watch-counters");

        var result = await response.ShouldHaveStatusCodeAndContent<List<DailyCategoryWatchCounterDto>>(
            HttpStatusCode.OK
        );
        result.Should().NotBeNull();
    }
}
