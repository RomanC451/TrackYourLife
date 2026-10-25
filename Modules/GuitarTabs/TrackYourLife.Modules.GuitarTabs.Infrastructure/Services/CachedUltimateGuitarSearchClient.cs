using Microsoft.Extensions.Caching.Memory;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Services;

internal sealed class CachedUltimateGuitarSearchClient(
    UltimateGuitarSearchClient inner,
    IMemoryCache cache
) : IUltimateGuitarSearchClient
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public async Task<Result<GuitarTabSearchResponse>> SearchByTitleAsync(
        string value,
        int page,
        CancellationToken cancellationToken
    )
    {
        var key = CacheKey(value, page);
        if (cache.TryGetValue(key, out GuitarTabSearchResponse? cached) && cached is not null)
        {
            return Result.Success(cached);
        }

        var result = await inner.SearchByTitleAsync(value, page, cancellationToken);
        if (result.IsSuccess)
        {
            cache.Set(key, result.Value, CacheDuration);
        }

        return result;
    }

    public Task<Result<GuitarTabContentResponse>> GetTabAsync(
        Uri tabUrl,
        CancellationToken cancellationToken
    ) => inner.GetTabAsync(tabUrl, cancellationToken);

    private static string CacheKey(string value, int page) =>
        $"guitar-tabs:search:{value.Trim().ToLowerInvariant()}:{page}";
}
