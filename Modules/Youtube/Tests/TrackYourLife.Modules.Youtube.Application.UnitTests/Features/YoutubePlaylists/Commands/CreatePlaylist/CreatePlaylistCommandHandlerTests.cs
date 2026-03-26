using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.CreatePlaylist;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubePlaylists.Commands.CreatePlaylist;

public sealed class CreatePlaylistCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubePlaylistsRepository _youtubePlaylistsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CreatePlaylistCommandHandler _handler;

    public CreatePlaylistCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubePlaylistsRepository = Substitute.For<IYoutubePlaylistsRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new CreatePlaylistCommandHandler(
            _userIdentifierProvider,
            _youtubePlaylistsRepository,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_AddsPlaylistAndReturnsId()
    {
        var userId = UserId.NewId();
        var utc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utc);

        var command = new CreatePlaylistCommand("Favorites");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _youtubePlaylistsRepository
            .Received(1)
            .AddAsync(
                Arg.Is<YoutubePlaylist>(p => p.Name == "Favorites" && p.UserId == userId),
                Arg.Any<CancellationToken>()
            );
    }
}
