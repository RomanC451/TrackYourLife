using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetWatchHistory;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;
using TrackYourLife.SharedLib.Contracts.Common;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeVideos.Queries;

public sealed class GetWatchHistoryTests
{
    private readonly ISender _sender;
    private readonly GetWatchHistory _endpoint;

    public GetWatchHistoryTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetWatchHistory(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithPagedList()
    {
        var watchedAt = DateTime.UtcNow;
        var entry = new WatchedVideoHistoryEntry(
            Video: new YoutubeVideoPreview(
                VideoId: "video1",
                Title: "Video 1",
                ThumbnailUrl: "https://example.com/thumb.jpg",
                ChannelName: "Channel 1",
                ChannelId: "channel1",
                PublishedAt: DateTime.UtcNow,
                Duration: "PT5M",
                ViewCount: 1000,
                IsWatched: true
            ),
            VideoId: "video1",
            WatchedAtUtc: watchedAt
        );

        var pagedListResult = PagedList<WatchedVideoHistoryEntry>.FromSlice([entry], 1, 20, 1);
        pagedListResult.IsSuccess.Should().BeTrue();

        _sender
            .Send(Arg.Any<GetWatchHistoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(pagedListResult.Value)));

        var request = new GetWatchHistoryRequest { Page = 1, PageSize = 20 };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<PagedList<WatchedVideoHistoryEntry>>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Items.Should().HaveCount(1);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetWatchHistoryQuery>(q => q.Page == 1 && q.PageSize == 20),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenUsingDefaultRequest_ShouldPassDefaultPaginationToQuery()
    {
        var emptyPage = PagedList<WatchedVideoHistoryEntry>.FromSlice([], 1, 20, 0);
        emptyPage.IsSuccess.Should().BeTrue();

        _sender
            .Send(Arg.Any<GetWatchHistoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(emptyPage.Value)));

        var request = new GetWatchHistoryRequest();

        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetWatchHistoryQuery>(q => q.Page == 1 && q.PageSize == 20),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<GetWatchHistoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<PagedList<WatchedVideoHistoryEntry>>(error)));

        var request = new GetWatchHistoryRequest { Page = 1, PageSize = 20 };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
