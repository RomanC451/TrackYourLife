using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetYoutubeVideoDuration;

public sealed record GetYoutubeVideoDurationQuery(string VideoId) : IQuery<YoutubeVideoDuration>;
