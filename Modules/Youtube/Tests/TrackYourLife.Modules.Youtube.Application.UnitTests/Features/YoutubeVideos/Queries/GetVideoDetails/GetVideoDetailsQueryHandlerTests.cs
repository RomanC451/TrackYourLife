using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetVideoDetails;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.GetVideoDetails;

public sealed class GetVideoDetailsQueryHandlerTests
{
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IWatchedVideosRepository _watchedVideosRepository;
    private readonly GetVideoDetailsQueryHandler _handler;

    public GetVideoDetailsQueryHandlerTests()
    {
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _watchedVideosRepository = Substitute.For<IWatchedVideosRepository>();

        _handler = new GetVideoDetailsQueryHandler(
            _youtubeApiService,
            _userIdentifierProvider,
            _watchedVideosRepository
        );
    }

    [Fact]
    public async Task Handle_WhenVideoNotWatched_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var query = new GetVideoDetailsQuery(videoId);

        _userIdentifierProvider.UserId.Returns(userId);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns((WatchedVideo?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WatchedVideoErrors.NotFound(videoId));
        await _youtubeApiService
            .DidNotReceive()
            .GetVideoDetailsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenVideoWatched_ReturnsVideoDetails()
    {
        // Arrange
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var query = new GetVideoDetailsQuery(videoId);
        var watchedVideo = WatchedVideo.Create(
            WatchedVideoId.NewId(),
            userId,
            videoId,
            "channel-id",
            DateTime.UtcNow
        ).Value;
        var videoDetails = new YoutubeVideoDetails(
            videoId,
            "Test Video",
            "Description",
            "https://embed.url",
            "https://thumbnail.url",
            "Channel Name",
            "channel-id",
            DateTime.UtcNow,
            "PT10M",
            1000,
            100
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns(watchedVideo);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(videoDetails));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(videoDetails);
        await _youtubeApiService
            .Received(1)
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>());
    }
}
