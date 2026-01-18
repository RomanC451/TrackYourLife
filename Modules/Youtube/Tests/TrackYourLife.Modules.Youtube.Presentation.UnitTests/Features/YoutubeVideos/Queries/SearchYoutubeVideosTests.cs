using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.SearchYoutubeVideos;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeVideos.Queries;

public class SearchYoutubeVideosTests
{
    private readonly ISender _sender;
    private readonly SearchYoutubeVideos _endpoint;

    public SearchYoutubeVideosTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new SearchYoutubeVideos(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithVideos()
    {
        // Arrange
        var videos = new List<YoutubeVideoPreview>
        {
            new(
                VideoId: "video1",
                Title: "Test Video 1",
                ThumbnailUrl: "https://example.com/thumb1.jpg",
                ChannelName: "Channel 1",
                ChannelId: "UCtest123",
                PublishedAt: DateTime.UtcNow,
                Duration: "PT5M",
                ViewCount: 1000,
                IsWatched: false
            ),
            new(
                VideoId: "video2",
                Title: "Test Video 2",
                ThumbnailUrl: string.Empty,
                ChannelName: "Channel 2",
                ChannelId: "UCtest456",
                PublishedAt: DateTime.UtcNow.AddDays(-1),
                Duration: "PT10M",
                ViewCount: 2000,
                IsWatched: false
            ),
        };

        _sender
            .Send(Arg.Any<SearchYoutubeVideosQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos)));

        var request = new SearchYoutubeVideosRequest { Query = "test", MaxResults = 10 };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<IEnumerable<YoutubeVideoPreview>>>().Subject;
        okResult.Value.Should().NotBeNull();
        var videoList = okResult.Value!.ToList();
        videoList.Should().HaveCount(2);
        videoList[0].Title.Should().Be("Test Video 1");

        await _sender
            .Received(1)
            .Send(
                Arg.Is<SearchYoutubeVideosQuery>(q => q.Query == "test" && q.MaxResults == 10),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenMaxResultsNotSpecified_ShouldUseDefault()
    {
        // Arrange
        _sender
            .Send(Arg.Any<SearchYoutubeVideosQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    Result.Success<IEnumerable<YoutubeVideoPreview>>(
                        new List<YoutubeVideoPreview>()
                    )
                )
            );

        var request = new SearchYoutubeVideosRequest { Query = "test", MaxResults = 10 };

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<SearchYoutubeVideosQuery>(q => q.Query == "test" && q.MaxResults == 10),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<SearchYoutubeVideosQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IEnumerable<YoutubeVideoPreview>>(error)));

        var request = new SearchYoutubeVideosRequest { Query = "test", MaxResults = 5 };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
