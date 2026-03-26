using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.AddVideoToPlaylist;

public sealed record AddVideoToPlaylistCommand(YoutubePlaylistId YoutubePlaylistId, string VideoId)
    : ICommand;
