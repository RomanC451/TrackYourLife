using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Queries.GetPlaylistById;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubePlaylists.Queries.GetPlaylistById;

public sealed class GetPlaylistByIdQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubePlaylistsQuery _youtubePlaylistsQuery;
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly IWatchedVideosRepository _watchedVideosRepository;
    private readonly GetPlaylistByIdQueryHandler _handler;

    public GetPlaylistByIdQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubePlaylistsQuery = Substitute.For<IYoutubePlaylistsQuery>();
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _watchedVideosRepository = Substitute.For<IWatchedVideosRepository>();

        _handler = new GetPlaylistByIdQueryHandler(
            _userIdentifierProvider,
            _youtubePlaylistsQuery,
            _youtubeApiService,
            _watchedVideosRepository
        );
    }

    [Fact]
    public async Task Handle_WhenPlaylistMissing_ReturnsNotFound()
    {
        var userId = UserId.NewId();
        var playlistId = YoutubePlaylistId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        _youtubePlaylistsQuery
            .GetByIdAndUserIdAsync(playlistId, userId, Arg.Any<CancellationToken>())
            .Returns((YoutubePlaylistReadModel?)null);

        var result = await _handler.Handle(
            new GetPlaylistByIdQuery(playlistId),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubePlaylistErrors.NotFound(playlistId.Value));
    }

    [Fact]
    public async Task Handle_WhenPlaylistExists_ReturnsVideosOrderedWithPreviews()
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
        var v1 = new YoutubePlaylistVideoReadModel(
            YoutubePlaylistVideoId.NewId(),
            playlistId,
            "a",
            DateTime.UtcNow.AddMinutes(-1)
        );
        var v2 = new YoutubePlaylistVideoReadModel(
            YoutubePlaylistVideoId.NewId(),
            playlistId,
            "b",
            DateTime.UtcNow
        );

        var d1 = new YoutubeVideoDetails(
            "a",
            "Title A",
            "Desc",
            "https://embed",
            "https://thumb-a",
            "Ch",
            "ch1",
            DateTime.UtcNow,
            "1:00",
            100,
            5
        );
        var d2 = new YoutubeVideoDetails(
            "b",
            "Title B",
            "Desc",
            "https://embed",
            "https://thumb-b",
            "Ch",
            "ch1",
            DateTime.UtcNow,
            "2:00",
            200,
            5
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubePlaylistsQuery
            .GetByIdAndUserIdAsync(playlistId, userId, Arg.Any<CancellationToken>())
            .Returns(playlist);
        _youtubePlaylistsQuery
            .GetVideosByPlaylistIdOrderedAsync(playlistId, Arg.Any<CancellationToken>())
            .Returns(new List<YoutubePlaylistVideoReadModel> { v1, v2 }.AsReadOnly());

        _youtubeApiService
            .GetVideoDetailsAsync("a", Arg.Any<CancellationToken>())
            .Returns(Result.Success(d1));
        _youtubeApiService
            .GetVideoDetailsAsync("b", Arg.Any<CancellationToken>())
            .Returns(Result.Success(d2));

        _watchedVideosRepository
            .GetByUserIdAndVideoIdsAsync(userId, Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var result = await _handler.Handle(
            new GetPlaylistByIdQuery(playlistId),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value!.Videos.Should().HaveCount(2);
        result.Value.Videos[0].Video.YoutubeId.Should().Be("b");
        result.Value.Videos[1].Video.YoutubeId.Should().Be("a");
        result.Value.Videos[0].Preview.Should().NotBeNull();
        result.Value.Videos[0].Preview!.Title.Should().Be("Title B");
        result.Value.Videos[1].Preview!.Title.Should().Be("Title A");
    }
}
