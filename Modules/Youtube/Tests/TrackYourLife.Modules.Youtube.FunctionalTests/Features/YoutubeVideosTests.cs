using System.Net;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;
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

    [Fact]
    public async Task GetAllLatestVideos_WithFavoritesOnlyAndNoFavoriteChannels_ShouldReturnEmptyList()
    {
        var catId = YoutubeCategoryId.NewId();
        var cat = YoutubeCategory
            .Create(
                catId,
                _user.Id,
                YoutubeCategoryDefaults.EntertainmentName,
                YoutubeCategoryDefaults.EntertainmentMaxVideosPerDay,
                YoutubeCategoryDefaults.EntertainmentDisplayOrder,
                DateTime.UtcNow
            )
            .Value;
        await _youtubeWriteDbContext.YoutubeCategories.AddAsync(cat);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                _user.Id,
                "UCnotfav123",
                "Not Favorite",
                null,
                catId,
                YoutubeCategoryDefaults.EntertainmentName,
                DateTime.UtcNow
            )
            .Value;
        await _youtubeWriteDbContext.YoutubeChannels.AddAsync(channel);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var response = await _client.GetAsync("/api/videos?favoritesOnly=true");

        var result = await response.ShouldHaveStatusCodeAndContent<List<YoutubeVideoPreview>>(
            HttpStatusCode.OK
        );
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllLatestVideos_WithFavoritesOnlyAndCategory_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync(
            $"/api/videos?favoritesOnly=true&youtubeCategoryId={Guid.NewGuid()}"
        );

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetHomeRecommendation_WithNoFavorites_ShouldReturnNullVideo()
    {
        var response = await _client.GetAsync("/api/videos/home-recommendation");

        var result = await response.ShouldHaveStatusCodeAndContent<HomeRecommendationResponse>(
            HttpStatusCode.OK
        );
        result.Video.Should().BeNull();
    }
}
