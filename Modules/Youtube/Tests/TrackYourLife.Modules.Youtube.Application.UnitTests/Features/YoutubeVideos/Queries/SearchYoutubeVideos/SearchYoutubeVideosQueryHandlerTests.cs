using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.SearchYoutubeVideos;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.SearchYoutubeVideos;

public sealed class SearchYoutubeVideosQueryHandlerTests
{
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly SearchYoutubeVideosQueryHandler _handler;

    public SearchYoutubeVideosQueryHandlerTests()
    {
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _handler = new SearchYoutubeVideosQueryHandler(_youtubeApiService);
    }

    [Fact]
    public async Task Handle_WhenSearchSucceeds_ReturnsVideos()
    {
        // Arrange
        var query = new SearchYoutubeVideosQuery("test query", MaxResults: 10);
        var videos = new List<YoutubeVideoPreview>
        {
            new(
                "video-1",
                "Video 1",
                "thumbnail",
                "Channel 1",
                "channel-1",
                DateTime.UtcNow,
                "PT10M",
                1000
            )
        };

        _youtubeApiService
            .SearchVideosAsync(query.Query, query.MaxResults, Arg.Any<CancellationToken>())
            .Returns(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        await _youtubeApiService
            .Received(1)
            .SearchVideosAsync(query.Query, query.MaxResults, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSearchFails_ReturnsFailure()
    {
        // Arrange
        var query = new SearchYoutubeVideosQuery("test query", MaxResults: 10);
        var error = InfrastructureErrors.SupaBaseClient.ClientNotWorking;

        _youtubeApiService
            .SearchVideosAsync(query.Query, query.MaxResults, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IEnumerable<YoutubeVideoPreview>>(error));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
