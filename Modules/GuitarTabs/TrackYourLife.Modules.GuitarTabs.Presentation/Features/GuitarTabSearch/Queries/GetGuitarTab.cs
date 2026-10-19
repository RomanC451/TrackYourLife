using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.GetGuitarTab;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarTabSearch.Queries;

internal sealed class GetGuitarTabRequest
{
    [QueryParam]
    public string Url { get; init; } = string.Empty;
}

internal sealed class GetGuitarTab(ISender sender) : Endpoint<GetGuitarTabRequest, IResult>
{
    public override void Configure()
    {
        Get(ApiRoutes.GuitarTab);
        Description(x =>
            x.Produces<GuitarTabContentResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status502BadGateway)
        );
    }

    public override async Task<IResult> ExecuteAsync(GetGuitarTabRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new GetGuitarTabQuery(req.Url.Trim()))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(response => response);
    }
}
