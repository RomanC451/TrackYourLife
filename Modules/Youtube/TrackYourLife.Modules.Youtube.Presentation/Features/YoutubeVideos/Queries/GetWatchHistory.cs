using System.ComponentModel;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetWatchHistory;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;

internal sealed record GetWatchHistoryRequest
{
    [QueryParam, DefaultValue(1)]
    public int Page { get; init; } = 1;

    [QueryParam, DefaultValue(20)]
    public int PageSize { get; init; } = 20;
}

internal sealed class GetWatchHistory(ISender sender) : Endpoint<GetWatchHistoryRequest, IResult>
{
    public override void Configure()
    {
        Get("watch-history");
        Group<YoutubeVideosGroup>();
        Description(x =>
            x.Produces<PagedList<WatchedVideoHistoryEntry>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetWatchHistoryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetWatchHistoryQuery(req.Page, req.PageSize))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(pagedList => pagedList);
    }
}
