using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.UnitTests.Features.GuitarPlaylists;

public sealed class GuitarPlaylistTests
{
    private static readonly UserId UserId = UserId.NewId();
    private static readonly DateTime CreatedOn = new(2026, 10, 19, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WithValidName_ReturnsPlaylist()
    {
        var result = GuitarPlaylist.Create(
            GuitarPlaylistId.NewId(),
            UserId,
            "  Fingerstyle  ",
            CreatedOn
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Fingerstyle");
        result.Value.UserId.Should().Be(UserId);
        result.Value.CreatedOnUtc.Should().Be(CreatedOn);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_Fails(string name)
    {
        var result = GuitarPlaylist.Create(GuitarPlaylistId.NewId(), UserId, name, CreatedOn);

        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("Favorites")]
    [InlineData("favorites")]
    [InlineData("  FAVORITES  ")]
    public void Create_WithReservedName_Fails(string name)
    {
        var result = GuitarPlaylist.Create(GuitarPlaylistId.NewId(), UserId, name, CreatedOn);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GuitarPlaylistErrors.NameReserved);
    }

    [Fact]
    public void UpdateName_WithValidName_UpdatesPlaylist()
    {
        var playlist = GuitarPlaylist
            .Create(GuitarPlaylistId.NewId(), UserId, "Acoustic", CreatedOn)
            .Value;
        var modifiedOn = CreatedOn.AddMinutes(5);

        var result = playlist.UpdateName("  Blues  ", modifiedOn);

        result.IsSuccess.Should().BeTrue();
        playlist.Name.Should().Be("Blues");
        playlist.ModifiedOnUtc.Should().Be(modifiedOn);
    }

    [Fact]
    public void UpdateName_WithReservedName_Fails()
    {
        var playlist = GuitarPlaylist
            .Create(GuitarPlaylistId.NewId(), UserId, "Acoustic", CreatedOn)
            .Value;

        var result = playlist.UpdateName("Favorites", CreatedOn.AddMinutes(1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GuitarPlaylistErrors.NameReserved);
        playlist.Name.Should().Be("Acoustic");
    }
}
