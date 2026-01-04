using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.WatchedVideos;

public class WatchedVideoTests
{
    private readonly WatchedVideoId _id;
    private readonly UserId _userId;
    private readonly string _videoId;
    private readonly string _channelId;
    private readonly DateTime _watchedAtUtc;

    public WatchedVideoTests()
    {
        _id = WatchedVideoId.NewId();
        _userId = UserId.NewId();
        _videoId = "video123";
        _channelId = "channel456";
        _watchedAtUtc = DateTime.UtcNow;
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateWatchedVideo()
    {
        // Act
        var result = WatchedVideo.Create(_id, _userId, _videoId, _channelId, _watchedAtUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_id);
        result.Value.UserId.Should().Be(_userId);
        result.Value.VideoId.Should().Be(_videoId);
        result.Value.ChannelId.Should().Be(_channelId);
        result.Value.WatchedAtUtc.Should().Be(_watchedAtUtc);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        // Arrange
        var emptyId = WatchedVideoId.Empty;

        // Act
        var result = WatchedVideo.Create(emptyId, _userId, _videoId, _channelId, _watchedAtUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(WatchedVideo)}.{nameof(WatchedVideo.Id).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldFail()
    {
        // Arrange
        var emptyUserId = UserId.Empty;

        // Act
        var result = WatchedVideo.Create(_id, emptyUserId, _videoId, _channelId, _watchedAtUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(WatchedVideo)}.{nameof(WatchedVideo.UserId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyVideoId_ShouldFail()
    {
        // Arrange
        var emptyVideoId = string.Empty;

        // Act
        var result = WatchedVideo.Create(_id, _userId, emptyVideoId, _channelId, _watchedAtUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(WatchedVideo)}.{nameof(WatchedVideo.VideoId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithNullVideoId_ShouldFail()
    {
        // Arrange
        string? nullVideoId = null;

        // Act
        var result = WatchedVideo.Create(_id, _userId, nullVideoId!, _channelId, _watchedAtUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(WatchedVideo)}.{nameof(WatchedVideo.VideoId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyChannelId_ShouldFail()
    {
        // Arrange
        var emptyChannelId = string.Empty;

        // Act
        var result = WatchedVideo.Create(_id, _userId, _videoId, emptyChannelId, _watchedAtUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(WatchedVideo)}.{nameof(WatchedVideo.ChannelId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithNullChannelId_ShouldFail()
    {
        // Arrange
        string? nullChannelId = null;

        // Act
        var result = WatchedVideo.Create(_id, _userId, _videoId, nullChannelId!, _watchedAtUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(WatchedVideo)}.{nameof(WatchedVideo.ChannelId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithDefaultWatchedAtUtc_ShouldFail()
    {
        // Arrange
        var defaultDateTime = default(DateTime);

        // Act
        var result = WatchedVideo.Create(_id, _userId, _videoId, _channelId, defaultDateTime);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(WatchedVideo)}.{nameof(WatchedVideo.WatchedAtUtc).ToCapitalCase()}.Empty");
    }
}
