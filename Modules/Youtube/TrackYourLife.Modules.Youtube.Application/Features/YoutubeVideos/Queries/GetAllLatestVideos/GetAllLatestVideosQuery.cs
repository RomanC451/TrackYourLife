using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;

public sealed record GetAllLatestVideosQuery(
    int MaxResultsPerChannel = 5,
    YoutubeCategoryId? YoutubeCategoryId = null
) : IQuery<IEnumerable<Models.YoutubeVideoPreview>>;
