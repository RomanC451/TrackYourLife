using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.UpdatePlaylist;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubePlaylists.Commands.UpdatePlaylist;

public sealed class UpdatePlaylistCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubePlaylistsRepository _youtubePlaylistsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly UpdatePlaylistCommandHandler _handler;

    public UpdatePlaylistCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubePlaylistsRepository = Substitute.For<IYoutubePlaylistsRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new UpdatePlaylistCommandHandler(
            _userIdentifierProvider,
            _youtubePlaylistsRepository,
            _dateTimeProvider
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
            new UpdatePlaylistCommand(playlistId, "New name"),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubePlaylistErrors.NotFound(playlistId.Value));
        _youtubePlaylistsRepository.DidNotReceive().Update(Arg.Any<YoutubePlaylist>());
    }

    [Fact]
    public async Task Handle_WhenValid_UpdatesName()
    {
        var userId = UserId.NewId();
        var playlistId = YoutubePlaylistId.NewId();
        var modified = new DateTime(2025, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var playlist = YoutubePlaylist.Create(
            playlistId,
            userId,
            "Old",
            DateTime.UtcNow.AddDays(-1)
        ).Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(modified);
        _youtubePlaylistsRepository
            .GetByIdAndUserIdAsync(playlistId, userId, Arg.Any<CancellationToken>())
            .Returns(playlist);

        var result = await _handler.Handle(
            new UpdatePlaylistCommand(playlistId, "  New name  "),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        playlist.Name.Should().Be("New name");
        playlist.ModifiedOnUtc.Should().Be(modified);
        _youtubePlaylistsRepository.Received(1).Update(playlist);
    }
}
