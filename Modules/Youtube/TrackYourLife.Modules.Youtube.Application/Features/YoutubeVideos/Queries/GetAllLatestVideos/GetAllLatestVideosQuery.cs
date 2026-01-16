using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Domain.Core;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;

public sealed record GetAllLatestVideosQuery(
    VideoCategory? Category = null,
    int MaxResultsPerChannel = 5
) : IQuery<IEnumerable<YoutubeVideoPreview>>;
