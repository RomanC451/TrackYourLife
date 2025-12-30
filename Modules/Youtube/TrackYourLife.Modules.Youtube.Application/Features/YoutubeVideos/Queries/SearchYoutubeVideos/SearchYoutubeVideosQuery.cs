using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.SearchYoutubeVideos;

public sealed record SearchYoutubeVideosQuery(string Query, int MaxResults = 10)
    : IQuery<IEnumerable<YoutubeVideoPreview>>;

