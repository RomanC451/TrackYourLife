namespace TrackYourLife.Modules.Common.Domain.Features.Cookies;

public interface ICookieRepository
{
    Task<Cookie?> GetByNameAndDomainAsync(
        string name,
        string domain,
        CancellationToken cancellationToken
    );

    Task AddAsync(Cookie cookie, CancellationToken cancellationToken);

    void Update(Cookie cookie);
}
