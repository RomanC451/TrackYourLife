using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetVideoDetails;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;

internal sealed record GetVideoDetailsRequest
{
    public string VideoId { get; init; } = string.Empty;
}

internal sealed class GetVideoDetails(ISender sender)
    : Endpoint<GetVideoDetailsRequest, IResult>
{
    public override void Configure()
    {
        Get("{videoId}");
        Group<YoutubeVideosGroup>();
        Description(x =>
            x.Produces<YoutubeVideoDetails>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetVideoDetailsRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetVideoDetailsQuery(VideoId: req.VideoId))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(x => x);
    }
}

