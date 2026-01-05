using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Models;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Features;

[Collection("Youtube Integration Tests")]
public class YoutubeChannelsTests(YoutubeFunctionalTestWebAppFactory factory)
    : YoutubeBaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetChannelsByCategory_WithNoChannels_ShouldReturnEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/channels?category=Entertainment");

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<List<YoutubeChannelDto>>(
            HttpStatusCode.OK
        );
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetChannelsByCategory_WithChannels_ShouldReturnChannels()
    {
        // Arrange
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                UserId.Create(_user.Id.Value),
                youtubeChannelId: "UCtest123",
                name: "Test Channel",
                thumbnailUrl: null,
                category: VideoCategory.Entertainment,
                createdOnUtc: DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeChannels.AddAsync(channel);
        await _youtubeWriteDbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/channels?category=Entertainment");

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<List<YoutubeChannelDto>>(
            HttpStatusCode.OK
        );
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Test Channel");
        result[0].Category.Should().Be(VideoCategory.Entertainment);
    }

    [Fact]
    public async Task AddChannelToCategory_WithValidData_ShouldReturnCreated()
    {
        // Arrange - Using a well-known YouTube channel ID for testing
        // This is the official YouTube channel ID format (UC + 22 characters)
        var validChannelId = "UC_x5XG1OV2P6uZZ5FSM9Ttw"; // YouTube's official channel
        var request = new AddChannelToCategoryRequest(
            YoutubeChannelId: validChannelId,
            Category: VideoCategory.Entertainment
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/channels", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        // Verify channel was created
        var channel = await _youtubeWriteDbContext
            .YoutubeChannels.AsNoTracking()
            .FirstOrDefaultAsync(c => c.YoutubeChannelId == validChannelId);
        channel.Should().NotBeNull();
        channel!.Category.Should().Be(VideoCategory.Entertainment);
    }

    [Fact]
    public async Task AddChannelToCategory_WithDuplicateChannel_ShouldReturnBadRequest()
    {
        // Arrange
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                _user.Id,
                youtubeChannelId: "UCtest123",
                name: "Test Channel",
                thumbnailUrl: null,
                category: VideoCategory.Entertainment,
                createdOnUtc: DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeChannels.AddAsync(channel);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var request = new AddChannelToCategoryRequest(
            YoutubeChannelId: "UCtest123",
            Category: VideoCategory.Educational
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/channels", request);

        // Assert
        var error = await response.ShouldHaveStatusCodeAndContent<ProblemDetails>(
            HttpStatusCode.BadRequest
        );
        error.Should().NotBeNull();
        error!.Type.Should().Contain("AlreadyExists");
    }

    [Fact]
    public async Task RemoveChannel_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                _user.Id,
                youtubeChannelId: "UCtest123",
                name: "Test Channel",
                thumbnailUrl: null,
                category: VideoCategory.Entertainment,
                createdOnUtc: DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeChannels.AddAsync(channel);
        await _youtubeWriteDbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/channels/{channel.Id}");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Verify channel was deleted
        var deletedChannel = await _youtubeWriteDbContext
            .YoutubeChannels.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == channel.Id);
        deletedChannel.Should().BeNull();
    }

    [Fact]
    public async Task RemoveChannel_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = YoutubeChannelId.NewId();

        // Act
        var response = await _client.DeleteAsync($"/api/channels/{nonExistentId}");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}
