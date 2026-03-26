using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.RemoveVideoFromPlaylist;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubePlaylists.Commands.RemoveVideoFromPlaylist;

public sealed class RemoveVideoFromPlaylistCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubePlaylistsRepository _youtubePlaylistsRepository;
    private readonly IYoutubePlaylistVideosRepository _youtubePlaylistVideosRepository;
    private readonly RemoveVideoFromPlaylistCommandHandler _handler;

    public RemoveVideoFromPlaylistCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubePlaylistsRepository = Substitute.For<IYoutubePlaylistsRepository>();
        _youtubePlaylistVideosRepository = Substitute.For<IYoutubePlaylistVideosRepository>();

        _handler = new RemoveVideoFromPlaylistCommandHandler(
            _userIdentifierProvider,
            _youtubePlaylistsRepository,
            _youtubePlaylistVideosRepository
        );
    }

    [Fact]
    public async Task Handle_WhenPlaylistMissing_ReturnsNotFound()
    {
        var userId = UserId.NewId();
        var playlistId = YoutubePlaylistId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        _youtubePlaylistsRepository
            .GetByIdAndUserIdAsync(playlistId, userId, Arg.Any<CancellationToken>())
            .Returns((YoutubePlaylist?)null);

        var result = await _handler.Handle(
            new RemoveVideoFromPlaylistCommand(playlistId, "vid1"),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubePlaylistErrors.NotFound(playlistId.Value));
        _youtubePlaylistVideosRepository.DidNotReceive().Remove(Arg.Any<YoutubePlaylistVideo>());
    }

    [Fact]
    public async Task Handle_WhenVideoNotInPlaylist_ReturnsVideoNotInPlaylist()
    {
        var userId = UserId.NewId();
        var playlistId = YoutubePlaylistId.NewId();
        const string youtubeId = "vid1";
        var playlist = YoutubePlaylist.Create(
            playlistId,
            userId,
            "List",
            DateTime.UtcNow
        ).Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubePlaylistsRepository
            .GetByIdAndUserIdAsync(playlistId, userId, Arg.Any<CancellationToken>())
            .Returns(playlist);
        _youtubePlaylistVideosRepository
            .GetByPlaylistIdAndYoutubeIdAsync(playlistId, youtubeId, Arg.Any<CancellationToken>())
            .Returns((YoutubePlaylistVideo?)null);

        var result = await _handler.Handle(
            new RemoveVideoFromPlaylistCommand(playlistId, youtubeId),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubePlaylistErrors.VideoNotInPlaylist(youtubeId));
        _youtubePlaylistVideosRepository.DidNotReceive().Remove(Arg.Any<YoutubePlaylistVideo>());
    }

    [Fact]
    public async Task Handle_WhenValid_RemovesVideo()
    {
        var userId = UserId.NewId();
        var playlistId = YoutubePlaylistId.NewId();
        const string youtubeId = "vid1";
        var playlist = YoutubePlaylist.Create(
            playlistId,
            userId,
            "List",
            DateTime.UtcNow
        ).Value;
        var item = YoutubePlaylistVideo.Create(
            YoutubePlaylistVideoId.NewId(),
            playlistId,
            youtubeId,
            DateTime.UtcNow
        ).Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubePlaylistsRepository
            .GetByIdAndUserIdAsync(playlistId, userId, Arg.Any<CancellationToken>())
            .Returns(playlist);
        _youtubePlaylistVideosRepository
            .GetByPlaylistIdAndYoutubeIdAsync(playlistId, youtubeId, Arg.Any<CancellationToken>())
            .Returns(item);

        var result = await _handler.Handle(
            new RemoveVideoFromPlaylistCommand(playlistId, youtubeId),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        _youtubePlaylistVideosRepository.Received(1).Remove(item);
    }
}
