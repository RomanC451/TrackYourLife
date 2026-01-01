using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Commands.PlayVideo;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Commands;

internal sealed class PlayVideo(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Post("{videoId}/play");
        Group<YoutubeVideosGroup>();
        Description(x =>
            x.Produces<YoutubeVideoDetails>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
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
            .Create(new PlayVideoCommand(VideoId: videoId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(x => x);
    }
}
