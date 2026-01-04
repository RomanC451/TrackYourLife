using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyEntertainmentCounters.Models;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.DailyEntertainmentCounters.Models;

public class DailyEntertainmentCounterDtoExtensionsTests
{
    [Fact]
    public void ToDto_WithDailyEntertainmentCounter_ShouldMapCorrectly()
    {
        // Arrange
        var id = DailyEntertainmentCounterId.NewId();
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var videosWatchedCount = 5;

        var counter = DailyEntertainmentCounter.Create(id, userId, date, videosWatchedCount).Value;

        // Act
        var dto = counter.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Date.Should().Be(date);
        dto.VideosWatchedCount.Should().Be(videosWatchedCount);
    }

    [Fact]
    public void ToDto_WithDailyEntertainmentCounterReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var id = DailyEntertainmentCounterId.NewId();
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var videosWatchedCount = 10;

        var readModel = new DailyEntertainmentCounterReadModel(
            id,
            userId,
            date,
            videosWatchedCount
        );

        // Act
        var dto = readModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Date.Should().Be(date);
        dto.VideosWatchedCount.Should().Be(videosWatchedCount);
    }

    [Fact]
    public void ToDto_WithZeroVideosWatched_ShouldMapCorrectly()
    {
        // Arrange
        var id = DailyEntertainmentCounterId.NewId();
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var counter = DailyEntertainmentCounter.Create(id, userId, date, 0).Value;

        // Act
        var dto = counter.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Date.Should().Be(date);
        dto.VideosWatchedCount.Should().Be(0);
    }
}
