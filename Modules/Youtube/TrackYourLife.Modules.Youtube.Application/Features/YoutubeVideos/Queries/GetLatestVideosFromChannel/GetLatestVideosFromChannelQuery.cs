using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetLatestVideosFromChannel;

public sealed record GetLatestVideosFromChannelQuery(string ChannelId, int MaxResults = 10)
    : IQuery<IEnumerable<YoutubeVideoPreview>>;

