using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.YoutubeChannels;

public class YoutubeChannelTests
{
    private readonly YoutubeChannelId _id;
    private readonly UserId _userId;
    private readonly string _youtubeChannelId;
    private readonly string _name;
    private readonly string? _thumbnailUrl;
    private readonly VideoCategory _category;
    private readonly DateTime _createdOnUtc;

    public YoutubeChannelTests()
    {
        _id = YoutubeChannelId.NewId();
        _userId = UserId.NewId();
        _youtubeChannelId = "UC1234567890";
        _name = "Test Channel";
        _thumbnailUrl = "https://example.com/thumbnail.jpg";
        _category = VideoCategory.Entertainment;
        _createdOnUtc = DateTime.UtcNow;
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateChannel()
    {
        // Act
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            _youtubeChannelId,
            _name,
            _thumbnailUrl,
            _category,
            _createdOnUtc
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_id);
        result.Value.UserId.Should().Be(_userId);
        result.Value.YoutubeChannelId.Should().Be(_youtubeChannelId);
        result.Value.Name.Should().Be(_name);
        result.Value.ThumbnailUrl.Should().Be(_thumbnailUrl);
        result.Value.Category.Should().Be(_category);
        result.Value.CreatedOnUtc.Should().Be(_createdOnUtc);
        result.Value.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullThumbnailUrl_ShouldCreateChannel()
    {
        // Arrange
        string? nullThumbnailUrl = null;

        // Act
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            _youtubeChannelId,
            _name,
            nullThumbnailUrl,
            _category,
            _createdOnUtc
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ThumbnailUrl.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        // Arrange
        var emptyId = YoutubeChannelId.Empty;

        // Act
        var result = YoutubeChannel.Create(
            emptyId,
            _userId,
            _youtubeChannelId,
            _name,
            _thumbnailUrl,
            _category,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.Id).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldFail()
    {
        // Arrange
        var emptyUserId = UserId.Empty;

        // Act
        var result = YoutubeChannel.Create(
            _id,
            emptyUserId,
            _youtubeChannelId,
            _name,
            _thumbnailUrl,
            _category,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.UserId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyYoutubeChannelId_ShouldFail()
    {
        // Arrange
        var emptyYoutubeChannelId = string.Empty;

        // Act
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            emptyYoutubeChannelId,
            _name,
            _thumbnailUrl,
            _category,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.YoutubeChannelId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithNullYoutubeChannelId_ShouldFail()
    {
        // Arrange
        string? nullYoutubeChannelId = null;

        // Act
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            nullYoutubeChannelId!,
            _name,
            _thumbnailUrl,
            _category,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.YoutubeChannelId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyName_ShouldFail()
    {
        // Arrange
        var emptyName = string.Empty;

        // Act
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            _youtubeChannelId,
            emptyName,
            _thumbnailUrl,
            _category,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.Name).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithNullName_ShouldFail()
    {
        // Arrange
        string? nullName = null;

        // Act
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            _youtubeChannelId,
            nullName!,
            _thumbnailUrl,
            _category,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.Name).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithDefaultCreatedOnUtc_ShouldFail()
    {
        // Arrange
        var defaultDateTime = default(DateTime);

        // Act
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            _youtubeChannelId,
            _name,
            _thumbnailUrl,
            _category,
            defaultDateTime
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.CreatedOnUtc).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void UpdateCategory_WithValidCategory_ShouldUpdateCategory()
    {
        // Arrange
        var channel = YoutubeChannel
            .Create(_id, _userId, _youtubeChannelId, _name, _thumbnailUrl, _category, _createdOnUtc)
            .Value;
        var newCategory = VideoCategory.Educational;

        // Act
        var result = channel.UpdateCategory(newCategory);

        // Assert
        result.IsSuccess.Should().BeTrue();
        channel.Category.Should().Be(newCategory);
    }

    [Fact]
    public void UpdateCategory_WithSameCategory_ShouldUpdateCategory()
    {
        // Arrange
        var channel = YoutubeChannel
            .Create(_id, _userId, _youtubeChannelId, _name, _thumbnailUrl, _category, _createdOnUtc)
            .Value;

        // Act
        var result = channel.UpdateCategory(_category);

        // Assert
        result.IsSuccess.Should().BeTrue();
        channel.Category.Should().Be(_category);
    }
}
