using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylistSongs;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Application.UnitTests.Features.SavedGuitarTabs;

public sealed class GuitarTabLibraryTests
{
    private const string TabUrl = "https://tabs.ultimate-guitar.com/tab/bosquito/viu-chords-5059138";

    private readonly UserId _userId = UserId.NewId();
    private readonly IUserIdentifierProvider _userIdentifierProvider =
        Substitute.For<IUserIdentifierProvider>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly ISavedGuitarTabsRepository _savedGuitarTabsRepository =
        Substitute.For<ISavedGuitarTabsRepository>();
    private readonly IGuitarPlaylistSongsRepository _guitarPlaylistSongsRepository =
        Substitute.For<IGuitarPlaylistSongsRepository>();
    private readonly IGuitarPlaylistsRepository _guitarPlaylistsRepository =
        Substitute.For<IGuitarPlaylistsRepository>();
    private readonly GuitarTabLibrary _library;
    private SavedGuitarTab? _stored;

    public GuitarTabLibraryTests()
    {
        _userIdentifierProvider.UserId.Returns(_userId);
        _dateTimeProvider.UtcNow.Returns(new DateTime(2026, 10, 19, 12, 0, 0, DateTimeKind.Utc));
        _savedGuitarTabsRepository
            .AddAsync(Arg.Do<SavedGuitarTab>(tab => _stored = tab), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _savedGuitarTabsRepository
            .GetByUserAndUrlAsync(_userId, TabUrl, Arg.Any<CancellationToken>())
            .Returns(_ => _stored);
        _savedGuitarTabsRepository
            .GetByIdAsync(Arg.Any<SavedGuitarTabId>(), Arg.Any<CancellationToken>())
            .Returns(_ => _stored);
        _guitarPlaylistSongsRepository
            .ListByTabIdAsync(Arg.Any<SavedGuitarTabId>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _library = new GuitarTabLibrary(
            _userIdentifierProvider,
            _dateTimeProvider,
            _savedGuitarTabsRepository,
            _guitarPlaylistSongsRepository,
            _guitarPlaylistsRepository
        );
    }

    [Fact]
    public async Task SetFavorite_WhenHearted_SavesTheTabWithoutAPlaylist()
    {
        var result = await _library.SetFavoriteAsync(Details(), true, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsFavorite.Should().BeTrue();
        result.Value.PlaylistIds.Should().BeEmpty();
        _stored.Should().NotBeNull();
        _stored!.IsFavorite.Should().BeTrue();
        await _guitarPlaylistSongsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<GuitarPlaylistSong>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetFavorite_WhenUnheartedAndNotInAPlaylist_RemovesTheTab()
    {
        await _library.SetFavoriteAsync(Details(), true, CancellationToken.None);

        var result = await _library.SetFavoriteAsync(Details(), false, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsFavorite.Should().BeFalse();
        _savedGuitarTabsRepository.Received(1).Remove(_stored!);
    }

    [Fact]
    public async Task SetFavorite_WhenUnheartedWhileInAPlaylist_KeepsTheFavorite()
    {
        var playlistId = GuitarPlaylistId.NewId();
        await _library.SetFavoriteAsync(Details(), true, CancellationToken.None);
        var membership = GuitarPlaylistSong
            .Create(GuitarPlaylistSongId.NewId(), playlistId, _stored!.Id, _dateTimeProvider.UtcNow)
            .Value;
        _guitarPlaylistSongsRepository
            .ListByTabIdAsync(_stored.Id, Arg.Any<CancellationToken>())
            .Returns([membership]);

        var result = await _library.SetFavoriteAsync(Details(), false, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsFavorite.Should().BeTrue();
        result.Value.PlaylistIds.Should().Contain(playlistId.Value);
        _savedGuitarTabsRepository.DidNotReceive().Remove(Arg.Any<SavedGuitarTab>());
    }

    [Fact]
    public async Task SetPlaylists_WhenAddedToAPlaylist_AlsoFavoritesTheTab()
    {
        var playlist = OwnedPlaylist();
        _guitarPlaylistsRepository
            .GetByIdAndUserIdAsync(playlist.Id, _userId, Arg.Any<CancellationToken>())
            .Returns(playlist);

        var result = await _library.SetPlaylistsAsync(
            Details(),
            [playlist.Id],
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.IsFavorite.Should().BeTrue();
        result.Value.PlaylistIds.Should().Contain(playlist.Id.Value);
        _stored!.IsFavorite.Should().BeTrue();
        await _guitarPlaylistSongsRepository
            .Received(1)
            .AddAsync(Arg.Any<GuitarPlaylistSong>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetPlaylists_WhenCleared_RemovesTheTabFromFavorites()
    {
        await _library.SetFavoriteAsync(Details(), true, CancellationToken.None);

        var result = await _library.SetPlaylistsAsync(Details(), [], CancellationToken.None);

        result.Value.IsFavorite.Should().BeFalse();
        _savedGuitarTabsRepository.Received(1).Remove(_stored!);
    }

    [Fact]
    public async Task RemoveFromPlaylist_WhenItWasTheLastPlaylist_RemovesTheFavorite()
    {
        var playlist = OwnedPlaylist();
        await SaveInPlaylist(playlist);
        var membership = GuitarPlaylistSong
            .Create(GuitarPlaylistSongId.NewId(), playlist.Id, _stored!.Id, _dateTimeProvider.UtcNow)
            .Value;
        _guitarPlaylistSongsRepository
            .ListByTabIdAsync(_stored.Id, Arg.Any<CancellationToken>())
            .Returns([membership]);

        var result = await _library.RemoveFromPlaylistAsync(
            playlist.Id,
            _stored.Id,
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        _savedGuitarTabsRepository.Received(1).Remove(_stored);
        _guitarPlaylistSongsRepository.DidNotReceive().Remove(Arg.Any<GuitarPlaylistSong>());
    }

    [Fact]
    public async Task RemoveFromPlaylist_WhenAnotherPlaylistRemains_KeepsTheFavorite()
    {
        var playlist = OwnedPlaylist();
        var otherPlaylistId = GuitarPlaylistId.NewId();
        await SaveInPlaylist(playlist);
        var removed = GuitarPlaylistSong
            .Create(GuitarPlaylistSongId.NewId(), playlist.Id, _stored!.Id, _dateTimeProvider.UtcNow)
            .Value;
        var remaining = GuitarPlaylistSong
            .Create(GuitarPlaylistSongId.NewId(), otherPlaylistId, _stored.Id, _dateTimeProvider.UtcNow)
            .Value;
        _guitarPlaylistSongsRepository
            .ListByTabIdAsync(_stored.Id, Arg.Any<CancellationToken>())
            .Returns([removed, remaining]);

        await _library.RemoveFromPlaylistAsync(playlist.Id, _stored.Id, CancellationToken.None);

        _guitarPlaylistSongsRepository.Received(1).Remove(removed);
        _savedGuitarTabsRepository.DidNotReceive().Remove(Arg.Any<SavedGuitarTab>());
    }

    [Fact]
    public async Task RemoveSongsOnlyInPlaylist_RemovesTabsThatHaveNoOtherPlaylist()
    {
        var playlist = OwnedPlaylist();
        var otherPlaylistId = GuitarPlaylistId.NewId();
        var onlyHere = SavedGuitarTab
            .Create(
                SavedGuitarTabId.NewId(),
                _userId,
                TabUrl,
                "Viu",
                "Bosquito",
                "Chords",
                true,
                _dateTimeProvider.UtcNow
            )
            .Value;
        var shared = SavedGuitarTab
            .Create(
                SavedGuitarTabId.NewId(),
                _userId,
                "https://tabs.ultimate-guitar.com/tab/oasis/wonderwall-chords-6125",
                "Wonderwall",
                "Oasis",
                "Chords",
                true,
                _dateTimeProvider.UtcNow
            )
            .Value;
        var onlyMembership = GuitarPlaylistSong
            .Create(GuitarPlaylistSongId.NewId(), playlist.Id, onlyHere.Id, _dateTimeProvider.UtcNow)
            .Value;
        var sharedHere = GuitarPlaylistSong
            .Create(GuitarPlaylistSongId.NewId(), playlist.Id, shared.Id, _dateTimeProvider.UtcNow)
            .Value;
        var sharedElsewhere = GuitarPlaylistSong
            .Create(GuitarPlaylistSongId.NewId(), otherPlaylistId, shared.Id, _dateTimeProvider.UtcNow)
            .Value;
        _guitarPlaylistSongsRepository
            .ListByPlaylistIdAsync(playlist.Id, Arg.Any<CancellationToken>())
            .Returns([onlyMembership, sharedHere]);
        _guitarPlaylistSongsRepository
            .ListByTabIdsAsync(Arg.Any<IReadOnlyCollection<SavedGuitarTabId>>(), Arg.Any<CancellationToken>())
            .Returns([onlyMembership, sharedHere, sharedElsewhere]);
        _savedGuitarTabsRepository
            .GetByIdAsync(onlyHere.Id, Arg.Any<CancellationToken>())
            .Returns(onlyHere);
        _savedGuitarTabsRepository
            .GetByIdAsync(shared.Id, Arg.Any<CancellationToken>())
            .Returns(shared);

        await _library.RemoveSongsOnlyInPlaylistAsync(playlist.Id, CancellationToken.None);

        _savedGuitarTabsRepository.Received(1).Remove(onlyHere);
        _savedGuitarTabsRepository.DidNotReceive().Remove(shared);
    }

    [Fact]
    public async Task SetYoutubeUrl_SavesTheVideoWithoutFavoritingTheTab()
    {
        var result = await _library.SetYoutubeUrlAsync(
            Details(),
            "https://www.youtube.com/watch?v=RBumgq5yVrA",
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.IsFavorite.Should().BeFalse();
        result.Value.YoutubeUrl.Should().Be("https://www.youtube.com/watch?v=RBumgq5yVrA");
        _stored!.YoutubeUrl.Should().Be("https://www.youtube.com/watch?v=RBumgq5yVrA");
        _savedGuitarTabsRepository.DidNotReceive().Update(Arg.Any<SavedGuitarTab>());
    }

    [Fact]
    public async Task SetYoutubeUrl_WhenClearedAndTheTabIsOnlyAVideo_RemovesTheTab()
    {
        await _library.SetYoutubeUrlAsync(
            Details(),
            "https://youtu.be/RBumgq5yVrA",
            CancellationToken.None
        );

        var result = await _library.SetYoutubeUrlAsync(Details(), null, CancellationToken.None);

        result.Value.YoutubeUrl.Should().BeNull();
        _savedGuitarTabsRepository.Received(1).Remove(_stored!);
    }

    [Fact]
    public async Task RemoveFromPlaylist_WhenTheLastPlaylistHasASavedVideo_KeepsTheVideoAndLeavesFavorites()
    {
        var playlist = OwnedPlaylist();
        await SaveInPlaylist(playlist);
        _stored!.SetYoutubeUrl("https://www.youtube.com/watch?v=RBumgq5yVrA", _dateTimeProvider.UtcNow);
        var membership = GuitarPlaylistSong
            .Create(GuitarPlaylistSongId.NewId(), playlist.Id, _stored.Id, _dateTimeProvider.UtcNow)
            .Value;
        _guitarPlaylistSongsRepository
            .ListByTabIdAsync(_stored.Id, Arg.Any<CancellationToken>())
            .Returns([membership]);

        var result = await _library.RemoveFromPlaylistAsync(
            playlist.Id,
            _stored.Id,
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        _stored.IsFavorite.Should().BeFalse();
        _stored.YoutubeUrl.Should().Be("https://www.youtube.com/watch?v=RBumgq5yVrA");
        _guitarPlaylistSongsRepository.Received(1).Remove(membership);
        _savedGuitarTabsRepository.DidNotReceive().Remove(_stored);
    }

    private async Task SaveInPlaylist(GuitarPlaylist playlist)
    {
        _guitarPlaylistsRepository
            .GetByIdAndUserIdAsync(playlist.Id, _userId, Arg.Any<CancellationToken>())
            .Returns(playlist);
        await _library.SetPlaylistsAsync(Details(), [playlist.Id], CancellationToken.None);
    }

    private GuitarPlaylist OwnedPlaylist() =>
        GuitarPlaylist.Create(GuitarPlaylistId.NewId(), _userId, "Acoustic", _dateTimeProvider.UtcNow).Value;

    private static GuitarTabSaveDetails Details() => new(TabUrl, "Viu", "Bosquito", "Chords");
}
