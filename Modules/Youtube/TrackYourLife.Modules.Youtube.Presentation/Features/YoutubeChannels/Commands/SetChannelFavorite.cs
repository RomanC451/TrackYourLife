using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.SetChannelFavorite;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;

internal sealed record SetChannelFavoriteRequest(bool IsFavorite);

internal sealed class SetChannelFavorite(ISender sender) : Endpoint<SetChannelFavoriteRequest, IResult>
{
    public override void Configure()
    {
        Patch("{id}/favorite");
        Group<YoutubeChannelsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        SetChannelFavoriteRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new SetChannelFavoriteCommand(
                    YoutubeChannelId: Route<string>("id")!,
                    IsFavorite: req.IsFavorite
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
