namespace TrackYourLife.Modules.Common.Domain.Features.Cookies;

public interface ICookieQuery
{
    Task<IEnumerable<CookieReadModel>> GetCookiesByDomainAsync(
        string domain,
        CancellationToken cancellationToken
    );
}
