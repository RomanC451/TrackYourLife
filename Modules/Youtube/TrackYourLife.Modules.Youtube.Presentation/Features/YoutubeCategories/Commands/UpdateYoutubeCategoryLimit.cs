using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryLimit;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeCategories.Commands;

internal sealed record UpdateYoutubeCategoryLimitRequest(int MaxVideosPerDay);

internal sealed class UpdateYoutubeCategoryLimit(ISender sender)
    : Endpoint<UpdateYoutubeCategoryLimitRequest, IResult>
{
    public override void Configure()
    {
        Put("{id:guid}/limit");
        Group<YoutubeCategoriesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateYoutubeCategoryLimitRequest req,
        CancellationToken ct
    )
    {
        var youtubeCategoryId = YoutubeCategoryId.Create(Route<Guid>("id"));

        return await Result
            .Create(
                new UpdateYoutubeCategoryLimitCommand(youtubeCategoryId, req.MaxVideosPerDay)
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
