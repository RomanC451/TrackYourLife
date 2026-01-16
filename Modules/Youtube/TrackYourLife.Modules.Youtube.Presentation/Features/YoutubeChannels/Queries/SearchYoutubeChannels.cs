using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.SearchYoutubeChannels;
using TrackYourLife.Modules.Youtube.Contracts.Dtos;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Queries;

internal sealed record SearchYoutubeChannelsRequest
{
    [QueryParam]
    public string Query { get; init; } = string.Empty;

    [QueryParam]
    public int MaxResults { get; init; } = 10;
}

internal sealed class SearchYoutubeChannels(ISender sender)
    : Endpoint<SearchYoutubeChannelsRequest, IResult>
{
    public override void Configure()
    {
        Get("search");
        Group<YoutubeChannelsGroup>();
        Description(x =>
            x.Produces<IEnumerable<YoutubeChannelSearchResult>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        SearchYoutubeChannelsRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new SearchYoutubeChannelsQuery(Query: req.Query, MaxResults: req.MaxResults))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(x => x);
    }
}
