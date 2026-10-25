using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetYoutubeVideoDuration;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.GetYoutubeVideoDuration;

public sealed class GetYoutubeVideoDurationQueryHandlerTests
{
    private readonly IYoutubeApiService _youtubeApiService = Substitute.For<IYoutubeApiService>();
    private readonly GetYoutubeVideoDurationQueryHandler _handler;

    public GetYoutubeVideoDurationQueryHandlerTests()
    {
        _handler = new GetYoutubeVideoDurationQueryHandler(_youtubeApiService);
    }

    [Fact]
    public async Task Handle_WhenDurationIsFound_ReturnsSeconds()
    {
        _youtubeApiService
            .GetVideoDurationSecondsAsync("V1OfCmY-ZRw", Arg.Any<CancellationToken>())
            .Returns(Result.Success(252));

        var result = await _handler.Handle(
            new GetYoutubeVideoDurationQuery("V1OfCmY-ZRw"),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.DurationSeconds.Should().Be(252);
    }

    [Fact]
    public async Task Handle_WhenVideoIsMissing_ReturnsFailure()
    {
        var error = new Error("Youtube.VideoNotFound", "missing", 404);
        _youtubeApiService
            .GetVideoDurationSecondsAsync("V1OfCmY-ZRw", Arg.Any<CancellationToken>())
            .Returns(Result.Failure<int>(error));

        var result = await _handler.Handle(
            new GetYoutubeVideoDurationQuery("V1OfCmY-ZRw"),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
