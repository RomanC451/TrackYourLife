using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;
using TrackYourLife.Modules.Youtube.Domain.Core;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;

internal sealed record GetAllLatestVideosRequest
{
    [QueryParam]
    public VideoCategory? Category { get; init; }

    [QueryParam]
    public int MaxResultsPerChannel { get; init; } = 5;
}

internal sealed class GetAllLatestVideos(ISender sender)
    : Endpoint<GetAllLatestVideosRequest, IResult>
{
    public override void Configure()
    {
        Get("");
        Group<YoutubeVideosGroup>();
        Description(x =>
            x.Produces<IEnumerable<YoutubeVideoPreview>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetAllLatestVideosRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new GetAllLatestVideosQuery(
                    Category: req.Category,
                    MaxResultsPerChannel: req.MaxResultsPerChannel
                )
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(x => x);
    }
}

