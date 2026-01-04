using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetVideoDetails;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;
using TrackYourLife.Modules.Youtube.Presentation.UnitTests;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeVideos.Queries;

public class GetVideoDetailsTests
{
    private readonly ISender _sender;
    private readonly GetVideoDetails _endpoint;

    public GetVideoDetailsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetVideoDetails(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithVideoDetails()
    {
        // Arrange
        var videoId = "dQw4w9WgXcQ";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "videoId", videoId } };
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
            .Send(Arg.Any<GetVideoDetailsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(videoDetails)));

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<YoutubeVideoDetails>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.VideoId.Should().Be(videoId);
        okResult.Value.Title.Should().Be("Test Video");

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetVideoDetailsQuery>(q => q.VideoId == videoId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenVideoIdIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "videoId", string.Empty } };
        _endpoint.SetHttpContext(httpContext);

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Should().BeOfType<BadRequest<string>>().Subject;
        badRequestResult.Value.Should().Be("Video ID is required.");

        await _sender
            .DidNotReceive()
            .Send(Arg.Any<GetVideoDetailsQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenVideoIdIsNull_ShouldReturnBadRequest()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "videoId", (string?)null } };
        _endpoint.SetHttpContext(httpContext);

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Should().BeOfType<BadRequest<string>>().Subject;
        badRequestResult.Value.Should().Be("Video ID is required.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var videoId = "dQw4w9WgXcQ";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "videoId", videoId } };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Video not found");
        _sender
            .Send(Arg.Any<GetVideoDetailsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<YoutubeVideoDetails>(error)));

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
