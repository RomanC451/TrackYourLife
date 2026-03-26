using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.DeletePlaylist;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubePlaylists.Commands.DeletePlaylist;

public sealed class DeletePlaylistCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubePlaylistsRepository _youtubePlaylistsRepository;
    private readonly DeletePlaylistCommandHandler _handler;

    public DeletePlaylistCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubePlaylistsRepository = Substitute.For<IYoutubePlaylistsRepository>();

        _handler = new DeletePlaylistCommandHandler(
            _userIdentifierProvider,
            _youtubePlaylistsRepository
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
            new DeletePlaylistCommand(playlistId),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubePlaylistErrors.NotFound(playlistId.Value));
        _youtubePlaylistsRepository.DidNotReceive().Remove(Arg.Any<YoutubePlaylist>());
    }

    [Fact]
    public async Task Handle_WhenValid_RemovesPlaylist()
    {
        var userId = UserId.NewId();
        var playlistId = YoutubePlaylistId.NewId();
        var playlist = YoutubePlaylist.Create(
            playlistId,
            userId,
            "To delete",
            DateTime.UtcNow
        ).Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubePlaylistsRepository
            .GetByIdAndUserIdAsync(playlistId, userId, Arg.Any<CancellationToken>())
            .Returns(playlist);

        var result = await _handler.Handle(
            new DeletePlaylistCommand(playlistId),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        _youtubePlaylistsRepository.Received(1).Remove(playlist);
    }
}
