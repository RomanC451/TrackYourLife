using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Models;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Queries.GetPlaylists;

public sealed record GetPlaylistsQuery : IQuery<IReadOnlyList<YoutubePlaylistWithVideoPreviews>>;
