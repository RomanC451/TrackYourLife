using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using TrackYourLife.Modules.Youtube.Infrastructure.Options;
using TrackYourLife.Modules.Youtube.Infrastructure.Services;

namespace TrackYourLife.Modules.Youtube.Infrastructure.UnitTests.Services;

public class PipedApiClientTests
{
    private static IOptions<YoutubeApiOptions> CreateOptions() =>
        Microsoft.Extensions.Options.Options.Create(
            new YoutubeApiOptions
            {
                PipedApiBaseUrl = "http://localhost:8080",
                PipedProxyBaseUrl = "http://localhost:8081",
                PipedFrontendBaseUrl = "http://localhost:3000",
                ApiKey = "unused",
            }
        );

    private static HttpClient CreateHttpClient(MockHttpMessageHandler mockHttp)
    {
        return new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost:8080/") };
    }

    [Fact]
    public async Task GetPlaybackInfoAsync_WhenHlsIsPresent_ShouldReturnAbsoluteHlsUrl()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Get, "http://localhost:8080/streams/video-1")
            .Respond("application/json", """{ "hls": "http://cdn.test/stream.m3u8" }""");

        var sut = new PipedApiClient(CreateHttpClient(mockHttp), CreateOptions());

        // Act
        var result = await sut.GetPlaybackInfoAsync("video-1", CancellationToken.None);

        // Assert
        result.DirectPlaybackUrl.Should().Be("http://cdn.test/stream.m3u8");
        result
            .EmbedUrl.Should()
            .Be("http://localhost:3000/embed/video-1?autoplay=true&quality=best");
    }

    [Fact]
    public async Task GetPlaybackInfoAsync_WhenBestStreamIsRelative_ShouldResolveAgainstProxyBaseUrl()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Get, "http://localhost:8080/streams/video-2")
            .Respond(
                "application/json",
                """
                {
                  "videoStreams": [
                    { "url": "/proxy/low.mp4", "bitrate": 2000, "videoOnly": false },
                    { "url": "/proxy/high.mp4", "bitrate": 5000, "videoOnly": false },
                    { "url": "/proxy/video-only.mp4", "bitrate": 9000, "videoOnly": true }
                  ]
                }
                """
            );

        var sut = new PipedApiClient(CreateHttpClient(mockHttp), CreateOptions());

        // Act
        var result = await sut.GetPlaybackInfoAsync("video-2", CancellationToken.None);

        // Assert
        result.DirectPlaybackUrl.Should().Be("http://localhost:8081/proxy/high.mp4");
        result
            .EmbedUrl.Should()
            .Be("http://localhost:3000/embed/video-2?autoplay=true&quality=best");
    }

    [Fact]
    public async Task GetPlaybackInfoAsync_WhenApiCallFails_ShouldFallbackToEmbedUrlOnly()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Get, "http://localhost:8080/streams/video-3")
            .Respond(System.Net.HttpStatusCode.BadGateway);

        var sut = new PipedApiClient(CreateHttpClient(mockHttp), CreateOptions());

        // Act
        var result = await sut.GetPlaybackInfoAsync("video-3", CancellationToken.None);

        // Assert
        result.DirectPlaybackUrl.Should().BeNull();
        result
            .EmbedUrl.Should()
            .Be("http://localhost:3000/embed/video-3?autoplay=true&quality=best");
    }

    [Fact]
    public async Task GetPlaybackInfoAsync_WhenVideoIdContainsReservedChars_ShouldEscapeInEmbedUrl()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Get, "http://localhost:8080/streams/*")
            .Respond(System.Net.HttpStatusCode.InternalServerError);

        var sut = new PipedApiClient(CreateHttpClient(mockHttp), CreateOptions());

        // Act
        var result = await sut.GetPlaybackInfoAsync("video/with space", CancellationToken.None);

        // Assert
        result.DirectPlaybackUrl.Should().BeNull();
        result
            .EmbedUrl.Should()
            .Be("http://localhost:3000/embed/video%2Fwith space?autoplay=true&quality=best");
    }
}
