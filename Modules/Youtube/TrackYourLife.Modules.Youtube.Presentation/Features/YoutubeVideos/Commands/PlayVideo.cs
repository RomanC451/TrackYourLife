using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Commands.PlayVideo;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Commands;

internal sealed class PlayVideo(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Post("{id}/play");
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
        return await Result
            .Create(new PlayVideoCommand(VideoId: Route<string>("id")!))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(x => x);
    }
}
