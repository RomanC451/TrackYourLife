using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.DailyEntertainmentCounters;

public class DailyEntertainmentCounterTests
{
    private readonly DailyEntertainmentCounterId _id;
    private readonly UserId _userId;
    private readonly DateOnly _date;

    public DailyEntertainmentCounterTests()
    {
        _id = DailyEntertainmentCounterId.NewId();
        _userId = UserId.NewId();
        _date = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateCounter()
    {
        // Arrange
        var videosWatchedCount = 5;

        // Act
        var result = DailyEntertainmentCounter.Create(_id, _userId, _date, videosWatchedCount);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_id);
        result.Value.UserId.Should().Be(_userId);
        result.Value.Date.Should().Be(_date);
        result.Value.VideosWatchedCount.Should().Be(videosWatchedCount);
    }

    [Fact]
    public void Create_WithDefaultVideosWatchedCount_ShouldCreateWithZero()
    {
        // Act
        var result = DailyEntertainmentCounter.Create(_id, _userId, _date);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.VideosWatchedCount.Should().Be(0);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        // Arrange
        var emptyId = DailyEntertainmentCounterId.Empty;

        // Act
        var result = DailyEntertainmentCounter.Create(emptyId, _userId, _date);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be(
                $"{nameof(DailyEntertainmentCounter)}.{nameof(DailyEntertainmentCounter.Id)}.Empty"
            );
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldFail()
    {
        // Arrange
        var emptyUserId = UserId.Empty;

        // Act
        var result = DailyEntertainmentCounter.Create(_id, emptyUserId, _date);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be(
                $"{nameof(DailyEntertainmentCounter)}.{nameof(DailyEntertainmentCounter.UserId)}.Empty"
            );
    }

    [Fact]
    public void Create_WithNegativeVideosWatchedCount_ShouldFail()
    {
        // Arrange
        var negativeCount = -1;

        // Act
        var result = DailyEntertainmentCounter.Create(_id, _userId, _date, negativeCount);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.DailyEntertainmentCounter.InvalidCount");
    }

    [Fact]
    public void Increment_ShouldIncreaseVideosWatchedCount()
    {
        // Arrange
        var counter = DailyEntertainmentCounter.Create(_id, _userId, _date, 5).Value;
        var initialCount = counter.VideosWatchedCount;

        // Act
        var result = counter.Increment();

        // Assert
        result.IsSuccess.Should().BeTrue();
        counter.VideosWatchedCount.Should().Be(initialCount + 1);
    }

    [Fact]
    public void CanWatchVideo_WhenCountIsLessThanMax_ShouldReturnTrue()
    {
        // Arrange
        var counter = DailyEntertainmentCounter.Create(_id, _userId, _date, 3).Value;
        var maxVideosPerDay = 5;

        // Act
        var result = counter.CanWatchVideo(maxVideosPerDay);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanWatchVideo_WhenCountEqualsMax_ShouldReturnFalse()
    {
        // Arrange
        var maxVideosPerDay = 5;
        var counter = DailyEntertainmentCounter.Create(_id, _userId, _date, maxVideosPerDay).Value;

        // Act
        var result = counter.CanWatchVideo(maxVideosPerDay);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanWatchVideo_WhenCountIsGreaterThanMax_ShouldReturnFalse()
    {
        // Arrange
        var maxVideosPerDay = 5;
        var counter = DailyEntertainmentCounter.Create(_id, _userId, _date, 6).Value;

        // Act
        var result = counter.CanWatchVideo(maxVideosPerDay);

        // Assert
        result.Should().BeFalse();
    }
}
