using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.CreatePlaylist;

public sealed record CreatePlaylistCommand(string Name) : ICommand<GuitarPlaylistId>;
