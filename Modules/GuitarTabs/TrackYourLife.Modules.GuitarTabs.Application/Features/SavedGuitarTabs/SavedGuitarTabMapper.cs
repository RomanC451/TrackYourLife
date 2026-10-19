using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;

internal static class SavedGuitarTabMapper
{
    public static SavedGuitarTabResponse Map(SavedGuitarTabReadModel tab) =>
        new(tab.Id.Value, tab.SongName, tab.ArtistName, tab.Type, tab.TabUrl);
}
