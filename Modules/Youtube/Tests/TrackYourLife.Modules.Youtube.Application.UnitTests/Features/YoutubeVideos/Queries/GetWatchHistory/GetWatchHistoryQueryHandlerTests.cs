using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetWatchHistory;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.GetWatchHistory;

public sealed class GetWatchHistoryQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IWatchedVideosRepository _watchedVideosRepository;
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly GetWatchHistoryQueryHandler _handler;
    private readonly UserId _userId = UserId.Create(Guid.NewGuid());

    public GetWatchHistoryQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _watchedVideosRepository = Substitute.For<IWatchedVideosRepository>();
        _youtubeApiService = Substitute.For<IYoutubeApiService>();

        _userIdentifierProvider.UserId.Returns(_userId);

        _handler = new GetWatchHistoryQueryHandler(
            _userIdentifierProvider,
            _watchedVideosRepository,
            _youtubeApiService
        );
    }

    [Fact]
    public async Task Handle_WhenNoHistory_ShouldReturnEmptyPagedList()
    {
        _watchedVideosRepository
            .GetPagedByUserIdAsync(_userId, 1, 20, Arg.Any<CancellationToken>())
            .Returns((Array.Empty<WatchedVideo>(), 0));

        var result = await _handler.Handle(new GetWatchHistoryQuery(1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.Page.Should().Be(1);
        result.Value.MaxPage.Should().Be(1);

        await _youtubeApiService
            .DidNotReceive()
            .GetVideoPreviewsByIdsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenHistoryExists_ShouldReturnEntriesWithPreviews()
    {
        var watchedAt = DateTime.UtcNow.AddHours(-2);
        var watchedVideo = WatchedVideo
            .Create(
                WatchedVideoId.NewId(),
                _userId,
                "video1",
                "channel1",
                watchedAt
            )
            .Value;

        _watchedVideosRepository
            .GetPagedByUserIdAsync(_userId, 1, 20, Arg.Any<CancellationToken>())
            .Returns((new List<WatchedVideo> { watchedVideo }, 1));

        var preview = new YoutubeVideoPreview(
            VideoId: "video1",
            Title: "Video 1",
            ThumbnailUrl: "https://example.com/thumb.jpg",
            ChannelName: "Channel 1",
            ChannelId: "channel1",
            PublishedAt: DateTime.UtcNow.AddDays(-1),
            Duration: "PT5M",
            ViewCount: 1000,
            IsWatched: true
        );

        _youtubeApiService
            .GetVideoPreviewsByIdsAsync(
                Arg.Is<IEnumerable<string>>(ids => ids.Single() == "video1"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Result.Success<IReadOnlyDictionary<string, YoutubeVideoPreview>>(
                    new Dictionary<string, YoutubeVideoPreview> { ["video1"] = preview }
                )
            );

        var result = await _handler.Handle(new GetWatchHistoryQuery(1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        var entry = result.Value.Items.Single();
        entry.VideoId.Should().Be("video1");
        entry.Video.Should().Be(preview);
        entry.WatchedAtUtc.Should().Be(watchedAt);
    }

    [Fact]
    public async Task Handle_WhenPreviewMissing_ShouldReturnEntryWithNullVideo()
    {
        var watchedAt = DateTime.UtcNow.AddDays(-1);
        var watchedVideo = WatchedVideo
            .Create(
                WatchedVideoId.NewId(),
                _userId,
                "missing-video",
                "channel1",
                watchedAt
            )
            .Value;

        _watchedVideosRepository
            .GetPagedByUserIdAsync(_userId, 1, 20, Arg.Any<CancellationToken>())
            .Returns((new List<WatchedVideo> { watchedVideo }, 1));

        _youtubeApiService
            .GetVideoPreviewsByIdsAsync(
                Arg.Is<IEnumerable<string>>(ids => ids.Single() == "missing-video"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Result.Success<IReadOnlyDictionary<string, YoutubeVideoPreview>>(
                    new Dictionary<string, YoutubeVideoPreview>()
                )
            );

        var result = await _handler.Handle(new GetWatchHistoryQuery(1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var entry = result.Value.Items.Single();
        entry.VideoId.Should().Be("missing-video");
        entry.Video.Should().BeNull();
        entry.WatchedAtUtc.Should().Be(watchedAt);
    }

    [Fact]
    public async Task Handle_ShouldPassPageAndPageSizeToRepository()
    {
        _watchedVideosRepository
            .GetPagedByUserIdAsync(_userId, 2, 10, Arg.Any<CancellationToken>())
            .Returns((Array.Empty<WatchedVideo>(), 0));

        await _handler.Handle(new GetWatchHistoryQuery(2, 10), CancellationToken.None);

        await _watchedVideosRepository
            .Received(1)
            .GetPagedByUserIdAsync(_userId, 2, 10, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenMultipleVideos_ShouldPreserveWatchedOrder()
    {
        var older = WatchedVideo
            .Create(
                WatchedVideoId.NewId(),
                _userId,
                "video-old",
                "channel1",
                DateTime.UtcNow.AddDays(-2)
            )
            .Value;
        var newer = WatchedVideo
            .Create(
                WatchedVideoId.NewId(),
                _userId,
                "video-new",
                "channel1",
                DateTime.UtcNow.AddHours(-1)
            )
            .Value;

        _watchedVideosRepository
            .GetPagedByUserIdAsync(_userId, 1, 20, Arg.Any<CancellationToken>())
            .Returns((new List<WatchedVideo> { newer, older }, 2));

        _youtubeApiService
            .GetVideoPreviewsByIdsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(
                Result.Success<IReadOnlyDictionary<string, YoutubeVideoPreview>>(
                    new Dictionary<string, YoutubeVideoPreview>()
                )
            );

        var result = await _handler.Handle(new GetWatchHistoryQuery(1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Select(x => x.VideoId).Should().Equal("video-new", "video-old");
    }

    [Fact]
    public async Task Handle_WhenPreviewLookupFails_ShouldReturnFailure()
    {
        var watchedVideo = WatchedVideo
            .Create(
                WatchedVideoId.NewId(),
                _userId,
                "video1",
                "channel1",
                DateTime.UtcNow
            )
            .Value;

        _watchedVideosRepository
            .GetPagedByUserIdAsync(_userId, 1, 20, Arg.Any<CancellationToken>())
            .Returns((new List<WatchedVideo> { watchedVideo }, 1));

        var error = new Error("Youtube.GetVideoPreviewsByIdsFailed", "Failed", 500);
        _youtubeApiService
            .GetVideoPreviewsByIdsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IReadOnlyDictionary<string, YoutubeVideoPreview>>(error));

        var result = await _handler.Handle(new GetWatchHistoryQuery(1, 20), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
