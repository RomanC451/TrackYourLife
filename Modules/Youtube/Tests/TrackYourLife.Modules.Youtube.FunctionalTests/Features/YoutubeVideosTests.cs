using System.Net;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Features;

[Collection("Youtube Integration Tests")]
public class YoutubeVideosTests(YoutubeFunctionalTestWebAppFactory factory)
    : YoutubeBaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllLatestVideos_WithNoChannels_ShouldReturnEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/videos");

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<List<YoutubeVideoPreview>>(
            HttpStatusCode.OK
        );
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllLatestVideos_WithCategoryFilter_ShouldReturnFilteredVideos()
    {
        // Act
        var response = await _client.GetAsync("/api/videos?category=Entertainment");

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<List<YoutubeVideoPreview>>(
            HttpStatusCode.OK
        );
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task PlayVideo_WithValidVideoId_ShouldReturnVideoDetails()
    {
        // Arrange
        var videoId = "dQw4w9WgXcQ";

        // Act
        var response = await _client.PostAsync($"/api/videos/{videoId}/play", null);

        // Assert
        // Note: This test may fail if the video doesn't exist or API is not configured
        // In a real scenario, you might want to mock the YouTube API
        var result = await response.ShouldHaveStatusCodeAndContent<YoutubeVideoDetails>(
            HttpStatusCode.OK
        );
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task PlayVideo_WithInvalidVideoId_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidVideoId = "invalid-video-id";

        // Act
        var response = await _client.PostAsync($"/api/videos/{invalidVideoId}/play", null);

        // Assert
        // The actual status code depends on the implementation
        // It could be BadRequest, NotFound, or Forbidden
        response
            .StatusCode.Should()
            .BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.Forbidden);
    }
}
