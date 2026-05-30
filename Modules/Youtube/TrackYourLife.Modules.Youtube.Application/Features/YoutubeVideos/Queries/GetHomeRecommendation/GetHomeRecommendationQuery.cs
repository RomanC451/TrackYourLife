using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetHomeRecommendation;

public sealed record GetHomeRecommendationQuery() : IQuery<YoutubeVideoPreview?>;
