using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetVideoDetails;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;

internal sealed class GetVideoDetails(ISender sender) : Endpoint<EmptyRequest, IResult>
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

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var videoId = Route<string>("videoId");

        if (string.IsNullOrEmpty(videoId))
        {
            return Results.BadRequest("Video ID is required.");
        }

        return await Result
            .Create(new GetVideoDetailsQuery(VideoId: videoId))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(x => x);
    }
}
