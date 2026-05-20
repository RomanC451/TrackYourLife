using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryMetadata;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeCategories.Commands;

internal sealed record UpdateYoutubeCategoryMetadataRequest(string Name, int DisplayOrder);

internal sealed class UpdateYoutubeCategoryMetadata(ISender sender)
    : Endpoint<UpdateYoutubeCategoryMetadataRequest, IResult>
{
    public override void Configure()
    {
        Put("{id:guid}");
        Group<YoutubeCategoriesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status409Conflict)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateYoutubeCategoryMetadataRequest req,
        CancellationToken ct
    )
    {
        var youtubeCategoryId = YoutubeCategoryId.Create(Route<Guid>("id"));

        return await Result
            .Create(
                new UpdateYoutubeCategoryMetadataCommand(
                    youtubeCategoryId,
                    req.Name,
                    req.DisplayOrder
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
