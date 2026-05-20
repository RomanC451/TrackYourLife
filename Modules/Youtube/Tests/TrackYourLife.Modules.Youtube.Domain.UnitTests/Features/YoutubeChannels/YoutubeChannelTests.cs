using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
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
    private readonly YoutubeCategoryId _categoryId;
    private readonly string _categoryName;
    private readonly DateTime _createdOnUtc;

    public YoutubeChannelTests()
    {
        _id = YoutubeChannelId.NewId();
        _userId = UserId.NewId();
        _youtubeChannelId = "UC1234567890";
        _name = "Test Channel";
        _thumbnailUrl = "https://example.com/thumbnail.jpg";
        _categoryId = YoutubeCategoryId.NewId();
        _categoryName = "Entertainment";
        _createdOnUtc = DateTime.UtcNow;
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateChannel()
    {
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            _youtubeChannelId,
            _name,
            _thumbnailUrl,
            _categoryId,
            _categoryName,
            _createdOnUtc
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_id);
        result.Value.UserId.Should().Be(_userId);
        result.Value.YoutubeChannelId.Should().Be(_youtubeChannelId);
        result.Value.Name.Should().Be(_name);
        result.Value.ThumbnailUrl.Should().Be(_thumbnailUrl);
        result.Value.YoutubeCategoryId.Should().Be(_categoryId);
        result.Value.CategoryName.Should().Be(_categoryName);
        result.Value.CreatedOnUtc.Should().Be(_createdOnUtc);
        result.Value.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullThumbnailUrl_ShouldCreateChannel()
    {
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            _youtubeChannelId,
            _name,
            null,
            _categoryId,
            _categoryName,
            _createdOnUtc
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.ThumbnailUrl.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        var result = YoutubeChannel.Create(
            YoutubeChannelId.Empty,
            _userId,
            _youtubeChannelId,
            _name,
            _thumbnailUrl,
            _categoryId,
            _categoryName,
            _createdOnUtc
        );

        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.Id).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldFail()
    {
        var result = YoutubeChannel.Create(
            _id,
            UserId.Empty,
            _youtubeChannelId,
            _name,
            _thumbnailUrl,
            _categoryId,
            _categoryName,
            _createdOnUtc
        );

        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.UserId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyYoutubeChannelId_ShouldFail()
    {
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            string.Empty,
            _name,
            _thumbnailUrl,
            _categoryId,
            _categoryName,
            _createdOnUtc
        );

        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.YoutubeChannelId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithNullYoutubeChannelId_ShouldFail()
    {
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            null!,
            _name,
            _thumbnailUrl,
            _categoryId,
            _categoryName,
            _createdOnUtc
        );

        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.YoutubeChannelId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyName_ShouldFail()
    {
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            _youtubeChannelId,
            string.Empty,
            _thumbnailUrl,
            _categoryId,
            _categoryName,
            _createdOnUtc
        );

        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.Name).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithNullName_ShouldFail()
    {
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            _youtubeChannelId,
            null!,
            _thumbnailUrl,
            _categoryId,
            _categoryName,
            _createdOnUtc
        );

        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.Name).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithDefaultCreatedOnUtc_ShouldFail()
    {
        var result = YoutubeChannel.Create(
            _id,
            _userId,
            _youtubeChannelId,
            _name,
            _thumbnailUrl,
            _categoryId,
            _categoryName,
            default
        );

        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeChannel)}.{nameof(YoutubeChannel.CreatedOnUtc).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void AssignCategory_WithValidCategory_ShouldUpdateCategory()
    {
        var channel = YoutubeChannel
            .Create(
                _id,
                _userId,
                _youtubeChannelId,
                _name,
                _thumbnailUrl,
                _categoryId,
                _categoryName,
                _createdOnUtc
            )
            .Value;
        var newId = YoutubeCategoryId.NewId();
        var utc = DateTime.UtcNow;

        var result = channel.AssignCategory(newId, "Educational", utc);

        result.IsSuccess.Should().BeTrue();
        channel.YoutubeCategoryId.Should().Be(newId);
        channel.CategoryName.Should().Be("Educational");
        channel.ModifiedOnUtc.Should().Be(utc);
    }

    [Fact]
    public void AssignCategory_WithSameName_ShouldSucceed()
    {
        var channel = YoutubeChannel
            .Create(
                _id,
                _userId,
                _youtubeChannelId,
                _name,
                _thumbnailUrl,
                _categoryId,
                _categoryName,
                _createdOnUtc
            )
            .Value;
        var utc = DateTime.UtcNow;

        var result = channel.AssignCategory(_categoryId, "  Entertainment  ", utc);

        result.IsSuccess.Should().BeTrue();
        channel.CategoryName.Should().Be("Entertainment");
    }
}
