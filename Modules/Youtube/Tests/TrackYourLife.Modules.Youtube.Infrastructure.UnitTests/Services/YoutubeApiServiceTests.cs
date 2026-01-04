using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Infrastructure.Options;
using TrackYourLife.Modules.Youtube.Infrastructure.Services;

namespace TrackYourLife.Modules.Youtube.Infrastructure.UnitTests.Services;

public class YoutubeApiServiceTests : IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly IOptions<YoutubeApiOptions> _options;
    private YoutubeApiService? _sut;
    private bool _disposed;

    public YoutubeApiServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _options = Microsoft.Extensions.Options.Options.Create(
            new YoutubeApiOptions
            {
                ApiKey = "test-api-key",
                SearchCacheDuration = TimeSpan.FromHours(1),
                ChannelVideosCacheDuration = TimeSpan.FromMinutes(30),
                VideoDetailsCacheDuration = TimeSpan.FromHours(2),
            }
        );
    }

    [Fact]
    public void Constructor_WithValidOptions_ShouldCreateInstance()
    {
        // Act
        _sut = new YoutubeApiService(_options, _memoryCache);

        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);

        // Act & Assert
        var act = () => _sut.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_MultipleTimes_ShouldNotThrow()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);

        // Act & Assert
        _sut.Dispose();
        var act = () => _sut.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public async Task SearchChannelsAsync_WithCachedResult_ShouldReturnCachedData()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var query = "test-query";
        var cacheKey = $"youtube:search:channels:{query.ToLowerInvariant()}:10";
        var cachedChannels = new List<YoutubeChannelSearchResult>
        {
            new(
                ChannelId: "channel1",
                Name: "Test Channel",
                Description: "Test Description",
                ThumbnailUrl: "https://example.com/thumb.jpg",
                SubscriberCount: 1000
            ),
        };

        _memoryCache.Set(
            cacheKey,
            cachedChannels.AsEnumerable(),
            _options.Value.SearchCacheDuration
        );

        // Act
        var result = await _sut.SearchChannelsAsync(query, 10, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cachedChannels);
    }

    [Fact]
    public async Task GetChannelVideosAsync_WithCachedResult_ShouldReturnCachedData()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var channelId = "test-channel-id";
        var cacheKey = $"youtube:channel:{channelId}:videos:10";
        var cachedVideos = new List<YoutubeVideoPreview>
        {
            new(
                VideoId: "video1",
                Title: "Test Video",
                ThumbnailUrl: "https://example.com/thumb.jpg",
                ChannelName: "Test Channel",
                ChannelId: channelId,
                PublishedAt: DateTime.UtcNow,
                Duration: "10:00",
                ViewCount: 1000
            ),
        };

        _memoryCache.Set(
            cacheKey,
            cachedVideos.AsEnumerable(),
            _options.Value.ChannelVideosCacheDuration
        );

        // Act
        var result = await _sut.GetChannelVideosAsync(channelId, 10, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cachedVideos);
    }

    [Fact]
    public async Task SearchVideosAsync_WithCachedResult_ShouldReturnCachedData()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var query = "test-query";
        var cacheKey = $"youtube:search:videos:{query.ToLowerInvariant()}:10";
        var cachedVideos = new List<YoutubeVideoPreview>
        {
            new(
                VideoId: "video1",
                Title: "Test Video",
                ThumbnailUrl: "https://example.com/thumb.jpg",
                ChannelName: "Test Channel",
                ChannelId: "channel1",
                PublishedAt: DateTime.UtcNow,
                Duration: "10:00",
                ViewCount: 1000
            ),
        };

        _memoryCache.Set(cacheKey, cachedVideos.AsEnumerable(), _options.Value.SearchCacheDuration);

        // Act
        var result = await _sut.SearchVideosAsync(query, 10, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cachedVideos);
    }

    [Fact]
    public async Task GetVideoDetailsAsync_WithCachedResult_ShouldReturnCachedData()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var videoId = "test-video-id";
        var cacheKey = $"youtube:video:{videoId}:details";
        var cachedDetails = new YoutubeVideoDetails(
            VideoId: videoId,
            Title: "Test Video",
            Description: "Test Description",
            EmbedUrl: $"https://www.youtube.com/embed/{videoId}",
            ThumbnailUrl: "https://example.com/thumb.jpg",
            ChannelName: "Test Channel",
            ChannelId: "channel1",
            PublishedAt: DateTime.UtcNow,
            Duration: "10:00",
            ViewCount: 1000,
            LikeCount: 100
        );

        _memoryCache.Set(cacheKey, cachedDetails, _options.Value.VideoDetailsCacheDuration);

        // Act
        var result = await _sut.GetVideoDetailsAsync(videoId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cachedDetails);
    }

    [Fact]
    public async Task GetChannelInfoAsync_WithCachedResult_ShouldReturnCachedData()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var channelId = "test-channel-id";
        var cacheKey = $"youtube:channel:{channelId}:info";
        var cachedChannel = new YoutubeChannelSearchResult(
            ChannelId: channelId,
            Name: "Test Channel",
            Description: "Test Description",
            ThumbnailUrl: "https://example.com/thumb.jpg",
            SubscriberCount: 1000
        );

        _memoryCache.Set(cacheKey, cachedChannel, _options.Value.SearchCacheDuration);

        // Act
        var result = await _sut.GetChannelInfoAsync(channelId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cachedChannel);
    }

    [Fact]
    public async Task GetVideosFromChannelsAsync_WithEmptyChannelList_ShouldReturnEmptyResult()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var channelIds = Enumerable.Empty<string>();

        // Act
        var result = await _sut.GetVideosFromChannelsAsync(channelIds, 5, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchChannelsAsync_WithoutCache_ShouldReturnFailureDueToInvalidApiKey()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var query = "test-query";
        // No cache set, so it will try to make API call

        // Act
        var result = await _sut.SearchChannelsAsync(query, 10, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.SearchChannelsFailed");
    }

    [Fact]
    public async Task GetChannelVideosAsync_WithoutCache_ShouldReturnFailureDueToInvalidApiKey()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var channelId = "test-channel-id";
        // No cache set, so it will try to make API call

        // Act
        var result = await _sut.GetChannelVideosAsync(channelId, 10, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.GetChannelVideosFailed");
    }

    [Fact]
    public async Task SearchVideosAsync_WithoutCache_ShouldReturnFailureDueToInvalidApiKey()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var query = "test-query";
        // No cache set, so it will try to make API call

        // Act
        var result = await _sut.SearchVideosAsync(query, 10, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.SearchVideosFailed");
    }

    [Fact]
    public async Task GetVideoDetailsAsync_WithoutCache_ShouldReturnFailureDueToInvalidApiKey()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var videoId = "test-video-id";
        // No cache set, so it will try to make API call

        // Act
        var result = await _sut.GetVideoDetailsAsync(videoId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.GetVideoDetailsFailed");
    }

    [Fact]
    public async Task GetChannelInfoAsync_WithoutCache_ShouldReturnFailureDueToInvalidApiKey()
    {
        // Arrange
        _sut = new YoutubeApiService(_options, _memoryCache);
        var channelId = "test-channel-id";
        // No cache set, so it will try to make API call

        // Act
        var result = await _sut.GetChannelInfoAsync(channelId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.GetChannelInfoFailed");
    }

    // Note: These tests verify error handling when API calls fail (e.g., invalid API key).
    // The service catches exceptions and returns Result.Failure with appropriate error codes.
    // For full API integration testing with valid responses, integration tests should be created.

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _sut?.Dispose();
                _memoryCache?.Dispose();
            }

            _disposed = true;
        }
    }
}
