namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.GetGuitarTab;

internal sealed class GetGuitarTabQueryHandler(IUltimateGuitarSearchClient searchClient)
    : IQueryHandler<GetGuitarTabQuery, GuitarTabContentResponse>
{
    public Task<Result<GuitarTabContentResponse>> Handle(
        GetGuitarTabQuery request,
        CancellationToken cancellationToken
    )
    {
        if (!UltimateGuitarTabUrl.TryCreate(request.Url, out var tabUrl))
        {
            return Task.FromResult(
                Result.Failure<GuitarTabContentResponse>(GuitarTabSearchErrors.InvalidTabUrl)
            );
        }

        return searchClient.GetTabAsync(tabUrl, cancellationToken);
    }
}
