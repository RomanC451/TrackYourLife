using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.UpdatePlaylist;

public sealed record UpdatePlaylistCommand(GuitarPlaylistId GuitarPlaylistId, string Name) : ICommand;
