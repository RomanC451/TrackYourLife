using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetVideoDetails;

public sealed record GetVideoDetailsQuery(string VideoId) : IQuery<YoutubeVideoDetails>;

