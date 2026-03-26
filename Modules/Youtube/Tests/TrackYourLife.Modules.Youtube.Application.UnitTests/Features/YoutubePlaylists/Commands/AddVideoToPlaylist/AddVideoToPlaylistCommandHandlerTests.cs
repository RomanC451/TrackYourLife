using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.AddVideoToPlaylist;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubePlaylists.Commands.AddVideoToPlaylist;

public sealed class AddVideoToPlaylistCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubePlaylistsRepository _youtubePlaylistsRepository;
    private readonly IYoutubePlaylistVideosRepository _youtubePlaylistVideosRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly AddVideoToPlaylistCommandHandler _handler;

    public AddVideoToPlaylistCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubePlaylistsRepository = Substitute.For<IYoutubePlaylistsRepository>();
        _youtubePlaylistVideosRepository = Substitute.For<IYoutubePlaylistVideosRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new AddVideoToPlaylistCommandHandler(
            _userIdentifierProvider,
            _youtubePlaylistsRepository,
            _youtubePlaylistVideosRepository,
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
            new AddVideoToPlaylistCommand(playlistId, "abc12345678"),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubePlaylistErrors.NotFound(playlistId.Value));
        await _youtubePlaylistVideosRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<YoutubePlaylistVideo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenVideoAlreadyInPlaylist_ReturnsVideoAlreadyInPlaylist()
    {
        var userId = UserId.NewId();
        var playlistId = YoutubePlaylistId.NewId();
        const string videoId = "abc12345678";
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
            .ExistsAsync(playlistId, videoId, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _handler.Handle(
            new AddVideoToPlaylistCommand(playlistId, videoId),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubePlaylistErrors.VideoAlreadyInPlaylist(videoId));
        await _youtubePlaylistVideosRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<YoutubePlaylistVideo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValid_AddsVideo()
    {
        var userId = UserId.NewId();
        var playlistId = YoutubePlaylistId.NewId();
        const string videoId = "abc12345678";
        var utc = new DateTime(2025, 3, 1, 12, 0, 0, DateTimeKind.Utc);
        var playlist = YoutubePlaylist.Create(
            playlistId,
            userId,
            "List",
            DateTime.UtcNow
        ).Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utc);
        _youtubePlaylistsRepository
            .GetByIdAndUserIdAsync(playlistId, userId, Arg.Any<CancellationToken>())
            .Returns(playlist);
        _youtubePlaylistVideosRepository
            .ExistsAsync(playlistId, videoId, Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _handler.Handle(
            new AddVideoToPlaylistCommand(playlistId, videoId),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        await _youtubePlaylistVideosRepository
            .Received(1)
            .AddAsync(
                Arg.Is<YoutubePlaylistVideo>(v =>
                    v.YoutubePlaylistId == playlistId && v.YoutubeId == videoId && v.AddedOnUtc == utc
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
