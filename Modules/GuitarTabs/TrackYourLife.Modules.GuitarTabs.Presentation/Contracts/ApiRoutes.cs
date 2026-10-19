namespace TrackYourLife.Modules.GuitarTabs.Presentation.Contracts;

internal static class ApiRoutes
{
    private const string Root = "api";
    public const string GuitarPlaylists = $"{Root}/guitar-tabs/playlists";
    public const string GuitarTabSearch = $"{Root}/guitar-tabs/search";
    public const string GuitarTab = $"{Root}/guitar-tabs/tabs";
    public const string SavedGuitarTabs = $"{Root}/guitar-tabs/saved-tabs";
    public const string GuitarTabFavorites = $"{Root}/guitar-tabs/favorites";
}
