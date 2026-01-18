using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeVideos.Queries;

public class GetAllLatestVideosTests
{
    private readonly ISender _sender;
    private readonly GetAllLatestVideos _endpoint;

    public GetAllLatestVideosTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetAllLatestVideos(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithVideos()
    {
        // Arrange
        var videos = new List<YoutubeVideoPreview>
        {
            new(
                VideoId: "video1",
                Title: "Video 1",
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
                Title: "Video 2",
                ThumbnailUrl: string.Empty,
                ChannelName: "Channel 2",
                ChannelId: "UCtest456",
                PublishedAt: DateTime.UtcNow.AddDays(-1),
                Duration: "PT10M",
                ViewCount: 2000,
                IsWatched: false
            )
        };

        _sender
            .Send(Arg.Any<GetAllLatestVideosQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos)));

        var request = new GetAllLatestVideosRequest
        {
            Category = VideoCategory.Entertainment,
            MaxResultsPerChannel = 5
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<IEnumerable<YoutubeVideoPreview>>>().Subject;
        okResult.Value.Should().NotBeNull();
        var videoList = okResult.Value!.ToList();
        videoList.Should().HaveCount(2);
        videoList[0].Title.Should().Be("Video 1");

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetAllLatestVideosQuery>(
                    q =>
                        q.Category == VideoCategory.Entertainment
                        && q.MaxResultsPerChannel == 5
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryIsNull_ShouldPassNullToQuery()
    {
        // Arrange
        _sender
            .Send(Arg.Any<GetAllLatestVideosQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<YoutubeVideoPreview>>(new List<YoutubeVideoPreview>())));

        var request = new GetAllLatestVideosRequest { Category = null, MaxResultsPerChannel = 3 };

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetAllLatestVideosQuery>(q => q.Category == null && q.MaxResultsPerChannel == 3),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<GetAllLatestVideosQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IEnumerable<YoutubeVideoPreview>>(error)));

        var request = new GetAllLatestVideosRequest { Category = VideoCategory.Educational };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
