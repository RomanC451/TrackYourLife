using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Queries.GetPlaylists;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubePlaylists.Queries.GetPlaylists;

public sealed class GetPlaylistsQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubePlaylistsQuery _youtubePlaylistsQuery;
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly IWatchedVideosRepository _watchedVideosRepository;
    private readonly GetPlaylistsQueryHandler _handler;

    public GetPlaylistsQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubePlaylistsQuery = Substitute.For<IYoutubePlaylistsQuery>();
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _watchedVideosRepository = Substitute.For<IWatchedVideosRepository>();

        _handler = new GetPlaylistsQueryHandler(
            _userIdentifierProvider,
            _youtubePlaylistsQuery,
            _youtubeApiService,
            _watchedVideosRepository
        );
    }

    [Fact]
    public async Task Handle_WhenNoPlaylists_ReturnsEmptyWithoutCallingYoutube()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        _youtubePlaylistsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<YoutubePlaylistReadModel>());

        var result = await _handler.Handle(new GetPlaylistsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        await _youtubeApiService
            .DidNotReceive()
            .GetVideoDetailsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _watchedVideosRepository
            .DidNotReceive()
            .GetByUserIdAndVideoIdsAsync(
                Arg.Any<UserId>(),
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenPlaylistHasVideos_BuildsPreviewsAndWatchedStatus()
    {
        var userId = UserId.NewId();
        var playlistId = YoutubePlaylistId.NewId();
        var playlist = new YoutubePlaylistReadModel(
            playlistId,
            userId,
            "List",
            DateTime.UtcNow,
            null
        );
        var videoId = "vid1";
        var videoRow = new YoutubePlaylistVideoReadModel(
            YoutubePlaylistVideoId.NewId(),
            playlistId,
            videoId,
            DateTime.UtcNow
        );
        var details = new YoutubeVideoDetails(
            videoId,
            "Title",
            "Desc",
            "https://embed",
            "https://thumb",
            "Ch",
            "ch1",
            DateTime.UtcNow,
            "1:00",
            100,
            5
        );
        var watched = WatchedVideo.Create(
            WatchedVideoId.NewId(),
            userId,
            videoId,
            "ch1",
            DateTime.UtcNow
        ).Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubePlaylistsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns([playlist]);
        _youtubePlaylistsQuery
            .GetVideosByPlaylistIdOrderedAsync(playlistId, Arg.Any<CancellationToken>())
            .Returns([videoRow]);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(details));
        _watchedVideosRepository
            .GetByUserIdAndVideoIdsAsync(userId, Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns([watched]);

        var result = await _handler.Handle(new GetPlaylistsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        var row = result.Value[0];
        row.VideoPreviews.Should().HaveCount(1);
        row.VideoPreviews[0].VideoId.Should().Be(videoId);
        row.VideoPreviews[0].Title.Should().Be("Title");
        row.VideoPreviews[0].IsWatched.Should().BeTrue();
    }
}
