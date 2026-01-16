using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.RemoveChannel;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;

internal sealed class RemoveChannel(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("{id}");
        Group<YoutubeChannelsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new RemoveChannelCommand(YoutubeChannelId: Route<string>("id")!))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
