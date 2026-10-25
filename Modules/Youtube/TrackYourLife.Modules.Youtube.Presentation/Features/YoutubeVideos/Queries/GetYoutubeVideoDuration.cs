using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetYoutubeVideoDuration;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;

internal sealed class GetYoutubeVideoDuration(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Get("{id}/duration");
        Group<YoutubeVideosGroup>();
        Description(x =>
            x.Produces<YoutubeVideoDuration>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var videoId = Route<string>("id");

        if (string.IsNullOrEmpty(videoId))
        {
            return Results.BadRequest("Video ID is required.");
        }

        return await Result
            .Create(new GetYoutubeVideoDurationQuery(VideoId: videoId))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(duration => duration);
    }
}
