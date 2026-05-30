using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetHomeRecommendation;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeVideos.Queries;

public class GetHomeRecommendationTests
{
    private readonly ISender _sender;
    private readonly GetHomeRecommendation _endpoint;

    public GetHomeRecommendationTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetHomeRecommendation(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceedsWithVideo_ShouldReturnOkWithResponse()
    {
        var video = new YoutubeVideoPreview(
            "video-1",
            "Video 1",
            "thumbnail",
            "Channel 1",
            "channel-1",
            DateTime.UtcNow,
            "PT10M",
            1000,
            IsWatched: false
        );

        _sender
            .Send(Arg.Any<GetHomeRecommendationQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<YoutubeVideoPreview?>(video)));

        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        var okResult = result.Should().BeOfType<Ok<HomeRecommendationResponse>>().Subject;
        okResult.Value!.Video.Should().Be(video);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceedsWithNull_ShouldReturnOkWithNullVideo()
    {
        _sender
            .Send(Arg.Any<GetHomeRecommendationQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<YoutubeVideoPreview?>(null)));

        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        var okResult = result.Should().BeOfType<Ok<HomeRecommendationResponse>>().Subject;
        okResult.Value!.Video.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<GetHomeRecommendationQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<YoutubeVideoPreview?>(error)));

        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
