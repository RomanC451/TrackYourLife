using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.RemoveChannel;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;

internal sealed record RemoveChannelRequest(YoutubeChannelId Id);

internal sealed class RemoveChannel(ISender sender) : Endpoint<RemoveChannelRequest, IResult>
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

    public override async Task<IResult> ExecuteAsync(RemoveChannelRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new RemoveChannelCommand(Id: req.Id))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}

