using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Models;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Queries.GetPlaylistById;

public sealed record GetPlaylistByIdQuery(YoutubePlaylistId YoutubePlaylistId)
    : IQuery<GetPlaylistByIdResult>;
