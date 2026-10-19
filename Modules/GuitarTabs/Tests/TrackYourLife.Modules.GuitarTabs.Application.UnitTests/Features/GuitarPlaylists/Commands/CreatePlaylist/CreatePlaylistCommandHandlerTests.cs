using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.CreatePlaylist;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Application.UnitTests.Features.GuitarPlaylists.Commands.CreatePlaylist;

public sealed class CreatePlaylistCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IGuitarPlaylistsRepository _guitarPlaylistsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CreatePlaylistCommandHandler _handler;

    public CreatePlaylistCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _guitarPlaylistsRepository = Substitute.For<IGuitarPlaylistsRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new CreatePlaylistCommandHandler(
            _userIdentifierProvider,
            _guitarPlaylistsRepository,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_AddsPlaylistAndReturnsId()
    {
        var userId = UserId.NewId();
        var utc = new DateTime(2026, 10, 19, 0, 0, 0, DateTimeKind.Utc);
        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utc);

        var result = await _handler.Handle(new CreatePlaylistCommand("Acoustic"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _guitarPlaylistsRepository
            .Received(1)
            .AddAsync(
                Arg.Is<GuitarPlaylist>(playlist => playlist.Name == "Acoustic" && playlist.UserId == userId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WithReservedName_DoesNotAddPlaylist()
    {
        _userIdentifierProvider.UserId.Returns(UserId.NewId());
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        var result = await _handler.Handle(
            new CreatePlaylistCommand("Favorites"),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GuitarPlaylistErrors.NameReserved);
        await _guitarPlaylistsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<GuitarPlaylist>(), Arg.Any<CancellationToken>());
    }
}
