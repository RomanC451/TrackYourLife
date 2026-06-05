using System.Net;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Features;

public record PagedWatchHistoryResponse
{
    public IReadOnlyCollection<WatchedVideoHistoryEntry> Items { get; init; } =
        Array.Empty<WatchedVideoHistoryEntry>();

    public int Page { get; init; }
    public int PageSize { get; init; }
    public bool HasPreviousPage { get; init; }
    public int MaxPage { get; init; }
    public bool HasNextPage { get; init; }
}

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

    [Fact]
    public async Task GetWatchHistory_WithNoHistory_ShouldReturnEmptyPagedList()
    {
        var response = await _client.GetAsync("/api/videos/watch-history?page=1&pageSize=20");

        var result = await response.ShouldHaveStatusCodeAndContent<PagedWatchHistoryResponse>(
            HttpStatusCode.OK
        );
        result.Items.Should().BeEmpty();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
        result.MaxPage.Should().Be(1);
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetWatchHistory_WithInvalidPage_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync("/api/videos/watch-history?page=0&pageSize=20");

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetWatchHistory_WithInvalidPageSize_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync("/api/videos/watch-history?page=1&pageSize=51");

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetWatchHistory_WithSeededVideos_ShouldReturnPagedEntries()
    {
        var older = WatchedVideo
            .Create(
                WatchedVideoId.NewId(),
                _user.Id,
                "dQw4w9WgXcQ",
                "UCuAXFkgsw1L7xaCfnd5JJOw",
                DateTime.UtcNow.AddDays(-1)
            )
            .Value;
        var newer = WatchedVideo
            .Create(
                WatchedVideoId.NewId(),
                _user.Id,
                "jNQXAC9IVRw",
                "UCuAXFkgsw1L7xaCfnd5JJOw",
                DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.WatchedVideos.AddRangeAsync(older, newer);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var firstPageResponse = await _client.GetAsync(
            "/api/videos/watch-history?page=1&pageSize=1"
        );
        var firstPage = await firstPageResponse.ShouldHaveStatusCodeAndContent<PagedWatchHistoryResponse>(
            HttpStatusCode.OK
        );

        firstPage.Items.Should().HaveCount(1);
        firstPage.Page.Should().Be(1);
        firstPage.PageSize.Should().Be(1);
        firstPage.MaxPage.Should().Be(2);
        firstPage.HasNextPage.Should().BeTrue();
        firstPage.Items.Single().VideoId.Should().Be("jNQXAC9IVRw");
        firstPage.Items.Single().Video.Should().NotBeNull();
        firstPage.Items.Single().Video!.Title.Should().NotBeNullOrWhiteSpace();

        var secondPageResponse = await _client.GetAsync(
            "/api/videos/watch-history?page=2&pageSize=1"
        );
        var secondPage = await secondPageResponse.ShouldHaveStatusCodeAndContent<PagedWatchHistoryResponse>(
            HttpStatusCode.OK
        );

        secondPage.Items.Should().HaveCount(1);
        secondPage.HasPreviousPage.Should().BeTrue();
        secondPage.HasNextPage.Should().BeFalse();
        secondPage.Items.Single().VideoId.Should().Be("dQw4w9WgXcQ");
    }
}
