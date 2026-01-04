using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Commands.PlayVideo;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeVideos.Commands;

public class PlayVideoTests
{
    private readonly ISender _sender;
    private readonly PlayVideo _endpoint;

    public PlayVideoTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new PlayVideo(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnOkWithVideoDetails()
    {
        // Arrange
        var videoId = "dQw4w9WgXcQ";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", videoId } };
        _endpoint.SetHttpContext(httpContext);

        var videoDetails = new YoutubeVideoDetails(
            VideoId: videoId,
            Title: "Test Video",
            Description: "Test Description",
            EmbedUrl: "https://youtube.com/embed/" + videoId,
            ThumbnailUrl: "https://example.com/thumb.jpg",
            ChannelName: "Test Channel",
            ChannelId: "UCtest123",
            PublishedAt: DateTime.UtcNow,
            Duration: "PT5M",
            ViewCount: 1000,
            LikeCount: 50
        );

        _sender
            .Send(Arg.Any<PlayVideoCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(videoDetails)));

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<YoutubeVideoDetails>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.VideoId.Should().Be(videoId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<PlayVideoCommand>(c => c.VideoId == videoId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenVideoIdIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", string.Empty } };
        _endpoint.SetHttpContext(httpContext);

        var validationError = new Error("VideoId", "Video ID is required.");
        var validationResult = ValidationResult<YoutubeVideoDetails>.WithErrors([validationError]);
        _sender
            .Send(Arg.Any<PlayVideoCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<YoutubeVideoDetails>>(validationResult));

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Should().BeOfType<BadRequest<ProblemDetails>>().Subject;
        badRequestResult.Value.Should().NotBeNull();
        badRequestResult.Value!.Detail.Should().Be("A validation problem occurred.");

        await _sender
            .Received(1)
            .Send(
                Arg.Is<PlayVideoCommand>(c => c.VideoId == string.Empty),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var videoId = "dQw4w9WgXcQ";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", videoId } };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("Forbidden", "Limit reached");
        _sender
            .Send(Arg.Any<PlayVideoCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<YoutubeVideoDetails>(error)));

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
