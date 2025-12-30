using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetLatestVideosFromChannel;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;

internal sealed record GetLatestVideosFromChannelRequest
{
    public string ChannelId { get; init; } = string.Empty;

    [QueryParam]
    public int MaxResults { get; init; } = 10;
}

internal sealed class GetLatestVideosFromChannel(ISender sender)
    : Endpoint<GetLatestVideosFromChannelRequest, IResult>
{
    public override void Configure()
    {
        Get("channels/{channelId}");
        Group<YoutubeVideosGroup>();
        Description(x =>
            x.Produces<IEnumerable<YoutubeVideoPreview>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetLatestVideosFromChannelRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new GetLatestVideosFromChannelQuery(
                    ChannelId: req.ChannelId,
                    MaxResults: req.MaxResults
                )
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(x => x);
    }
}

