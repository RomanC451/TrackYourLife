using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.SearchYoutubeVideos;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;

internal sealed record SearchYoutubeVideosRequest
{
    [QueryParam]
    public string Query { get; init; } = string.Empty;

    [QueryParam]
    public int MaxResults { get; init; } = 10;
}

internal sealed class SearchYoutubeVideos(ISender sender)
    : Endpoint<SearchYoutubeVideosRequest, IResult>
{
    public override void Configure()
    {
        Get("search");
        Group<YoutubeVideosGroup>();
        Description(x =>
            x.Produces<IEnumerable<YoutubeVideoPreview>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        SearchYoutubeVideosRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new SearchYoutubeVideosQuery(Query: req.Query, MaxResults: req.MaxResults)
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(x => x);
    }
}

