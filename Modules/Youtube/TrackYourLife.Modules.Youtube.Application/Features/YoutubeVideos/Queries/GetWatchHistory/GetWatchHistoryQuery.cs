using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetWatchHistory;

public sealed record GetWatchHistoryQuery(int Page, int PageSize)
    : IQuery<PagedList<WatchedVideoHistoryEntry>>;
