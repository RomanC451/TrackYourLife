using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Services;

internal sealed class UltimateGuitarSearchClient(HttpClient httpClient) : IUltimateGuitarSearchClient
{
    public async Task<Result<GuitarTabSearchResponse>> SearchByTitleAsync(
        string value,
        int page,
        CancellationToken cancellationToken
    )
    {
        var path =
            $"search.php?search_type=title&value={Uri.EscapeDataString(value)}&page={page}";

        try
        {
            using var response = await httpClient.GetAsync(path, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure<GuitarTabSearchResponse>(GuitarTabSearchErrors.Unavailable);
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);
            return UltimateGuitarSearchPageParser.Parse(html, value);
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<GuitarTabSearchResponse>(GuitarTabSearchErrors.Unavailable);
        }
    }

    public async Task<Result<GuitarTabContentResponse>> GetTabAsync(
        Uri tabUrl,
        CancellationToken cancellationToken
    )
    {
        try
        {
            using var response = await httpClient.GetAsync(tabUrl, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure<GuitarTabContentResponse>(GuitarTabSearchErrors.Unavailable);
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);
            return UltimateGuitarTabPageParser.Parse(html);
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<GuitarTabContentResponse>(GuitarTabSearchErrors.Unavailable);
        }
    }
}
