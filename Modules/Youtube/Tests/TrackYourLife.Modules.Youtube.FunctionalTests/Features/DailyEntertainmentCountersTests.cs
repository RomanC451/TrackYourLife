using System.Net;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyEntertainmentCounters.Models;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Features;

[Collection("Youtube Integration Tests")]
public class DailyEntertainmentCountersTests(YoutubeFunctionalTestWebAppFactory factory)
    : YoutubeBaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetDailyEntertainmentCounter_WithNoCounter_ShouldReturnNull()
    {
        // Act
        var response = await _client.GetAsync("/api/settings/daily-counter");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
        {
            // Empty response means null
            return;
        }
        // If content exists, it should be null JSON
        var result = System.Text.Json.JsonSerializer.Deserialize<DailyEntertainmentCounterDto>(content);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDailyEntertainmentCounter_WithExistingCounter_ShouldReturnCounter()
    {
        // Arrange
        var counter = DailyEntertainmentCounter
            .Create(
                DailyEntertainmentCounterId.NewId(),
                _user.Id,
                date: DateOnly.FromDateTime(DateTime.UtcNow),
                videosWatchedCount: 3
            )
            .Value;

        await _youtubeWriteDbContext.DailyEntertainmentCounters.AddAsync(counter);
        await _youtubeWriteDbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/settings/daily-counter");

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<DailyEntertainmentCounterDto>(
            HttpStatusCode.OK
        );
        result.Should().NotBeNull();
        result!.VideosWatchedCount.Should().Be(3);
        result.Date.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
