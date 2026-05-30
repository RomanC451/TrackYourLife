using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetLatestVideosFromChannel;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.GetLatestVideosFromChannel;

public sealed class GetLatestVideosFromChannelQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly IWatchedVideosRepository _watchedVideosRepository;
    private readonly GetLatestVideosFromChannelQueryHandler _handler;

    public GetLatestVideosFromChannelQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _watchedVideosRepository = Substitute.For<IWatchedVideosRepository>();
        _handler = new GetLatestVideosFromChannelQueryHandler(
            _userIdentifierProvider,
            _youtubeApiService,
            _watchedVideosRepository
        );
    }

    [Fact]
    public async Task Handle_WhenVideosRetrieved_ReturnsVideos()
    {
        // Arrange
        var userId = UserId.NewId();
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
                1000,
                false
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeApiService
            .GetChannelVideosAsync(channelId, query.MaxResults, Arg.Any<CancellationToken>())
            .Returns(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos));
        _watchedVideosRepository
            .GetByUserIdAndVideoIdsAsync(
                userId,
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Enumerable.Empty<WatchedVideo>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.Single().IsWatched.Should().BeFalse();
        await _youtubeApiService
            .Received(1)
            .GetChannelVideosAsync(channelId, query.MaxResults, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MarksWatchedVideos()
    {
        var userId = UserId.NewId();
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
                1000,
                false
            ),
        };
        var watched = WatchedVideo
            .Create(WatchedVideoId.NewId(), userId, "video-1", channelId, DateTime.UtcNow)
            .Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeApiService
            .GetChannelVideosAsync(channelId, query.MaxResults, Arg.Any<CancellationToken>())
            .Returns(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos));
        _watchedVideosRepository
            .GetByUserIdAndVideoIdsAsync(
                userId,
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<WatchedVideo> { watched });

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Single().IsWatched.Should().BeTrue();
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
