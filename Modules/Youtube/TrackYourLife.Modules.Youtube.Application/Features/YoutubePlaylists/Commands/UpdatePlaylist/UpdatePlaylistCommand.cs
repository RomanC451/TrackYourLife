using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.UpdatePlaylist;

public sealed record UpdatePlaylistCommand(YoutubePlaylistId YoutubePlaylistId, string Name)
    : ICommand;
