namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

public interface IUltimateGuitarSearchClient
{
    Task<Result<GuitarTabSearchResponse>> SearchByTitleAsync(
        string value,
        int page,
        CancellationToken cancellationToken
    );

    Task<Result<GuitarTabContentResponse>> GetTabAsync(
        Uri tabUrl,
        CancellationToken cancellationToken
    );
}
