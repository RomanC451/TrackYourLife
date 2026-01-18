using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetLatestVideosFromChannel;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeVideos.Queries;

public class GetLatestVideosFromChannelTests
{
    private readonly ISender _sender;
    private readonly GetLatestVideosFromChannel _endpoint;

    public GetLatestVideosFromChannelTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetLatestVideosFromChannel(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithVideos()
    {
        // Arrange
        var channelId = "UCtest123";

        var videos = new List<YoutubeVideoPreview>
        {
            new(
                VideoId: "video1",
                Title: "Video 1",
                ThumbnailUrl: "https://example.com/thumb1.jpg",
                ChannelName: "Test Channel",
                ChannelId: channelId,
                PublishedAt: DateTime.UtcNow,
                Duration: "PT5M",
                ViewCount: 1000,
                IsWatched: false
            ),
            new(
                VideoId: "video2",
                Title: "Video 2",
                ThumbnailUrl: string.Empty,
                ChannelName: "Test Channel",
                ChannelId: channelId,
                PublishedAt: DateTime.UtcNow.AddDays(-1),
                Duration: "PT10M",
                ViewCount: 2000,
                IsWatched: false
            ),
        };

        _sender
            .Send(Arg.Any<GetLatestVideosFromChannelQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos)));

        var request = new GetLatestVideosFromChannelRequest
        {
            ChannelId = channelId,
            MaxResults = 10,
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<IEnumerable<YoutubeVideoPreview>>>().Subject;
        okResult.Value.Should().NotBeNull();
        var videoList = okResult.Value!.ToList();
        videoList.Should().HaveCount(2);
        videoList[0].ChannelId.Should().Be(channelId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetLatestVideosFromChannelQuery>(q =>
                    q.ChannelId == channelId && q.MaxResults == 10
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenMaxResultsNotSpecified_ShouldUseDefault()
    {
        // Arrange
        var channelId = "UCtest456";

        _sender
            .Send(Arg.Any<GetLatestVideosFromChannelQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    Result.Success<IEnumerable<YoutubeVideoPreview>>(
                        new List<YoutubeVideoPreview>()
                    )
                )
            );

        var request = new GetLatestVideosFromChannelRequest
        {
            ChannelId = channelId,
            MaxResults = 10,
        };

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetLatestVideosFromChannelQuery>(q =>
                    q.ChannelId == channelId && q.MaxResults == 10
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var channelId = "UCtest789";

        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<GetLatestVideosFromChannelQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IEnumerable<YoutubeVideoPreview>>(error)));

        var request = new GetLatestVideosFromChannelRequest
        {
            ChannelId = channelId,
            MaxResults = 5,
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
