using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Models;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeChannels.Models;

public class YoutubeChannelDtoExtensionsTests
{
    [Fact]
    public void ToDto_WithYoutubeChannelReadModel_ShouldMapCorrectly()
    {
        var id = YoutubeChannelId.NewId();
        var userId = UserId.NewId();
        var youtubeChannelId = "UCtest123";
        var name = "Test Channel";
        var thumbnailUrl = "https://example.com/thumb.jpg";
        var categoryId = YoutubeCategoryId.NewId();
        var createdOnUtc = DateTime.UtcNow;
        var modifiedOnUtc = DateTime.UtcNow.AddHours(1);

        var readModel = new YoutubeChannelReadModel(
            id,
            userId,
            youtubeChannelId,
            name,
            thumbnailUrl,
            categoryId,
            "Entertainment",
            false,
            createdOnUtc,
            modifiedOnUtc
        );

        var dto = readModel.ToDto();

        dto.Id.Should().Be(id.Value);
        dto.YoutubeChannelId.Should().Be(youtubeChannelId);
        dto.Name.Should().Be(name);
        dto.ThumbnailUrl.Should().Be(thumbnailUrl);
        dto.YoutubeCategoryId.Should().Be(categoryId.Value);
        dto.CategoryName.Should().Be("Entertainment");
        dto.IsFavorite.Should().BeFalse();
        dto.CreatedOnUtc.Should().Be(createdOnUtc);
    }

    [Fact]
    public void ToDto_WhenIsFavorite_ShouldMapCorrectly()
    {
        var id = YoutubeChannelId.NewId();
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var readModel = new YoutubeChannelReadModel(
            id,
            userId,
            "UCtest789",
            "Favorite Channel",
            null,
            categoryId,
            "Entertainment",
            true,
            DateTime.UtcNow,
            null
        );

        var dto = readModel.ToDto();

        dto.IsFavorite.Should().BeTrue();
    }

    [Fact]
    public void ToDto_WithNullThumbnailUrl_ShouldMapCorrectly()
    {
        var id = YoutubeChannelId.NewId();
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var readModel = new YoutubeChannelReadModel(
            id,
            userId,
            "UCtest456",
            "Another Channel",
            null,
            categoryId,
            "Educational",
            false,
            DateTime.UtcNow,
            null
        );

        var dto = readModel.ToDto();

        dto.ThumbnailUrl.Should().BeNull();
        dto.CategoryName.Should().Be("Educational");
    }
}
