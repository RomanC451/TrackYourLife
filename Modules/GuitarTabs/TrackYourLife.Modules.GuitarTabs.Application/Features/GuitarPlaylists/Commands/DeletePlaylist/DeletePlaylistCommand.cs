using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.DeletePlaylist;

public sealed record DeletePlaylistCommand(GuitarPlaylistId GuitarPlaylistId) : ICommand;
