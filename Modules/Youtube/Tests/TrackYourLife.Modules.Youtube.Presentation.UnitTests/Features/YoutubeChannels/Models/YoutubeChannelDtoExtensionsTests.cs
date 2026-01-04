using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Models;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeChannels.Models;

public class YoutubeChannelDtoExtensionsTests
{
    [Fact]
    public void ToDto_WithYoutubeChannelReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var id = YoutubeChannelId.NewId();
        var userId = UserId.NewId();
        var youtubeChannelId = "UCtest123";
        var name = "Test Channel";
        var thumbnailUrl = "https://example.com/thumb.jpg";
        var category = VideoCategory.Entertainment;
        var createdOnUtc = DateTime.UtcNow;
        var modifiedOnUtc = DateTime.UtcNow.AddHours(1);

        var readModel = new YoutubeChannelReadModel(
            id,
            userId,
            youtubeChannelId,
            name,
            thumbnailUrl,
            category,
            createdOnUtc,
            modifiedOnUtc
        );

        // Act
        var dto = readModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(id.Value);
        dto.YoutubeChannelId.Should().Be(youtubeChannelId);
        dto.Name.Should().Be(name);
        dto.ThumbnailUrl.Should().Be(thumbnailUrl);
        dto.Category.Should().Be(category);
        dto.CreatedOnUtc.Should().Be(createdOnUtc);
    }

    [Fact]
    public void ToDto_WithNullThumbnailUrl_ShouldMapCorrectly()
    {
        // Arrange
        var id = YoutubeChannelId.NewId();
        var userId = UserId.NewId();
        var readModel = new YoutubeChannelReadModel(
            id,
            userId,
            "UCtest456",
            "Another Channel",
            null,
            VideoCategory.Educational,
            DateTime.UtcNow,
            null
        );

        // Act
        var dto = readModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.ThumbnailUrl.Should().BeNull();
        dto.Category.Should().Be(VideoCategory.Educational);
    }
}
