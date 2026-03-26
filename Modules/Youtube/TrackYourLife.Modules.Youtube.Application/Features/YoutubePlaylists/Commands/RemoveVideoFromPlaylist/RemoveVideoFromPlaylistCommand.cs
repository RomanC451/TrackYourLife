using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.RemoveVideoFromPlaylist;

public sealed record RemoveVideoFromPlaylistCommand(YoutubePlaylistId YoutubePlaylistId, string YoutubeId)
    : ICommand;
