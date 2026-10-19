using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

public static class GuitarPlaylistErrors
{
    public const string FavoritesName = "Favorites";

    public static readonly Func<Guid, Error> NotFound = playlistId =>
        new(
            "GuitarPlaylists.NotFound",
            $"Playlist with id '{playlistId}' was not found.",
            404
        );

    public static readonly Error NameReserved =
        new(
            "GuitarPlaylists.NameReserved",
            "Favorites is a built-in page and cannot be used as a playlist name.",
            409
        );
}
