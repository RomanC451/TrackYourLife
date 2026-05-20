using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.MoveChannelToCategory;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;

internal sealed record MoveChannelToCategoryRequest(Guid YoutubeCategoryId);

internal sealed class MoveChannelToCategory(ISender sender) : Endpoint<MoveChannelToCategoryRequest, IResult>
{
    public override void Configure()
    {
        Patch("{id}/category");
        Group<YoutubeChannelsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        MoveChannelToCategoryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new MoveChannelToCategoryCommand(
                    YoutubeChannelId: Route<string>("id")!,
                    TargetYoutubeCategoryId: YoutubeCategoryId.Create(req.YoutubeCategoryId)
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
