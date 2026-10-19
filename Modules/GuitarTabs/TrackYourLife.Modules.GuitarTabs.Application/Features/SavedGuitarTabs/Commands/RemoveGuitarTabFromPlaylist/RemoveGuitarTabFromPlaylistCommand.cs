using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.RemoveGuitarTabFromPlaylist;

public sealed record RemoveGuitarTabFromPlaylistCommand(
    GuitarPlaylistId GuitarPlaylistId,
    SavedGuitarTabId SavedGuitarTabId
) : ICommand;
