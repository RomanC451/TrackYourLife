using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.CreatePlaylist;

public sealed record CreatePlaylistCommand(string Name) : ICommand<YoutubePlaylistId>;
