using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.AddChannelToCategory;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Presentation.Contracts;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;

internal sealed record AddChannelToCategoryRequest(
    string YoutubeChannelId,
    VideoCategory Category
);

internal sealed class AddChannelToCategory(ISender sender)
    : Endpoint<AddChannelToCategoryRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<YoutubeChannelsGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status409Conflict)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        AddChannelToCategoryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new AddChannelToCategoryCommand(
                    YoutubeChannelId: req.YoutubeChannelId,
                    Category: req.Category
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(channelId => $"/{ApiRoutes.Channels}/{channelId.Value}");
    }
}

