using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.DeleteYoutubeCategory;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeCategories.Commands;

internal sealed class DeleteYoutubeCategoryRequest
{
    [QueryParam]
    public bool ConfirmUnsubscribeChannels { get; set; }

    [QueryParam]
    public Guid? MoveChannelsToCategoryId { get; set; }
}

internal sealed class DeleteYoutubeCategory(ISender sender)
    : Endpoint<DeleteYoutubeCategoryRequest, IResult>
{
    public override void Configure()
    {
        Delete("{id:guid}");
        Group<YoutubeCategoriesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(DeleteYoutubeCategoryRequest req, CancellationToken ct)
    {
        var youtubeCategoryId = YoutubeCategoryId.Create(Route<Guid>("id"));

        return await Result
            .Create(
                new DeleteYoutubeCategoryCommand(
                    youtubeCategoryId,
                    req.ConfirmUnsubscribeChannels,
                    req.MoveChannelsToCategoryId is { } moveId
                        ? YoutubeCategoryId.Create(moveId)
                        : null
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
