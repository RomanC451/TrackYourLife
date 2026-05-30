using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;

internal sealed record GetAllLatestVideosRequest
{
    [QueryParam]
    public Guid? YoutubeCategoryId { get; init; }

    [QueryParam]
    public bool FavoritesOnly { get; init; }

    [QueryParam]
    public int MaxResultsPerChannel { get; init; } = 5;
}

internal sealed class GetAllLatestVideos(ISender sender)
    : Endpoint<GetAllLatestVideosRequest, IResult>
{
    public override void Configure()
    {
        Get("");
        Group<YoutubeVideosGroup>();
        Description(x =>
            x.Produces<IEnumerable<YoutubeVideoPreview>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetAllLatestVideosRequest req,
        CancellationToken ct
    )
    {
        YoutubeCategoryId? categoryId = req.YoutubeCategoryId is null
            ? null
            : YoutubeCategoryId.Create(req.YoutubeCategoryId.Value);

        return await Result
            .Create(
                new GetAllLatestVideosQuery(
                    MaxResultsPerChannel: req.MaxResultsPerChannel,
                    YoutubeCategoryId: categoryId,
                    FavoritesOnly: req.FavoritesOnly
                )
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(x => x);
    }
}
