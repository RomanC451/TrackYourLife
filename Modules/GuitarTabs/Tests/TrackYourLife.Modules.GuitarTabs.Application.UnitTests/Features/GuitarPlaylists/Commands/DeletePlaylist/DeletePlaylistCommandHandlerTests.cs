using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.DeletePlaylist;
using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Application.UnitTests.Features.GuitarPlaylists.Commands.DeletePlaylist;

public sealed class DeletePlaylistCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IGuitarPlaylistsRepository _guitarPlaylistsRepository;
    private readonly IGuitarTabLibrary _guitarTabLibrary;
    private readonly DeletePlaylistCommandHandler _handler;

    public DeletePlaylistCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _guitarPlaylistsRepository = Substitute.For<IGuitarPlaylistsRepository>();
        _guitarTabLibrary = Substitute.For<IGuitarTabLibrary>();
        _handler = new DeletePlaylistCommandHandler(
            _userIdentifierProvider,
            _guitarPlaylistsRepository,
            _guitarTabLibrary
        );
    }

    [Fact]
    public async Task Handle_WhenPlaylistIsMissing_ReturnsNotFound()
    {
        var userId = UserId.NewId();
        var playlistId = GuitarPlaylistId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        _guitarPlaylistsRepository
            .GetByIdAndUserIdAsync(playlistId, userId, Arg.Any<CancellationToken>())
            .Returns((GuitarPlaylist?)null);

        var result = await _handler.Handle(
            new DeletePlaylistCommand(playlistId),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(GuitarPlaylistErrors.NotFound(playlistId.Value).Code);
        _guitarPlaylistsRepository.DidNotReceive().Remove(Arg.Any<GuitarPlaylist>());
    }
}
