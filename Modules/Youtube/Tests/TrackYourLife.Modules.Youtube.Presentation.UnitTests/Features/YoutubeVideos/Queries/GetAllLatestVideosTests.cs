using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
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
        var catGuid = Guid.NewGuid();
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
        };

        _sender
            .Send(Arg.Any<GetAllLatestVideosQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos)));

        var request = new GetAllLatestVideosRequest { YoutubeCategoryId = catGuid, MaxResultsPerChannel = 5 };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<IEnumerable<YoutubeVideoPreview>>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Should().HaveCount(1);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetAllLatestVideosQuery>(q =>
                    q.YoutubeCategoryId == YoutubeCategoryId.Create(catGuid) && q.MaxResultsPerChannel == 5
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryIsNull_ShouldPassNullToQuery()
    {
        _sender
            .Send(Arg.Any<GetAllLatestVideosQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<YoutubeVideoPreview>>(new List<YoutubeVideoPreview>())));

        var request = new GetAllLatestVideosRequest { YoutubeCategoryId = null, MaxResultsPerChannel = 3 };

        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetAllLatestVideosQuery>(q => q.YoutubeCategoryId == null && q.MaxResultsPerChannel == 3),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<GetAllLatestVideosQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IEnumerable<YoutubeVideoPreview>>(error)));

        var request = new GetAllLatestVideosRequest { YoutubeCategoryId = Guid.NewGuid() };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
