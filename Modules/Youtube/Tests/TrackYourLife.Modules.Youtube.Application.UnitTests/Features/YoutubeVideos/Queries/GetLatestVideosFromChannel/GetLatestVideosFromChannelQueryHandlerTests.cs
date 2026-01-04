using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetLatestVideosFromChannel;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.GetLatestVideosFromChannel;

public sealed class GetLatestVideosFromChannelQueryHandlerTests
{
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly GetLatestVideosFromChannelQueryHandler _handler;

    public GetLatestVideosFromChannelQueryHandlerTests()
    {
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _handler = new GetLatestVideosFromChannelQueryHandler(_youtubeApiService);
    }

    [Fact]
    public async Task Handle_WhenVideosRetrieved_ReturnsVideos()
    {
        // Arrange
        var channelId = "channel-id";
        var query = new GetLatestVideosFromChannelQuery(channelId, MaxResults: 10);
        var videos = new List<YoutubeVideoPreview>
        {
            new(
                "video-1",
                "Video 1",
                "thumbnail",
                "Channel 1",
                channelId,
                DateTime.UtcNow,
                "PT10M",
                1000
            )
        };

        _youtubeApiService
            .GetChannelVideosAsync(channelId, query.MaxResults, Arg.Any<CancellationToken>())
            .Returns(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        await _youtubeApiService
            .Received(1)
            .GetChannelVideosAsync(channelId, query.MaxResults, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenApiFails_ReturnsFailure()
    {
        // Arrange
        var channelId = "channel-id";
        var query = new GetLatestVideosFromChannelQuery(channelId, MaxResults: 10);
        var error = InfrastructureErrors.SupaBaseClient.ClientNotWorking;

        _youtubeApiService
            .GetChannelVideosAsync(channelId, query.MaxResults, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IEnumerable<YoutubeVideoPreview>>(error));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
