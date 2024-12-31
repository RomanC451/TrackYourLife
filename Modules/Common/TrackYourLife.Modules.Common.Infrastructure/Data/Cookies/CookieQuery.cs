using System.Collections;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.Modules.Common.Infrastructure.Data.Cookies.Specifications;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Common.Infrastructure.Data.Cookies;

internal sealed class CookieQuery(CommonReadDbContext dbContext)
    : GenericQuery<CookieReadModel, CookieId>(dbContext.Cookies),
        ICookieQuery
{
    public async Task<IEnumerable<CookieReadModel>> GetCookiesByDomainAsync(
        string domain,
        CancellationToken cancellationToken
    )
    {
        return await WhereAsync(new CookieReadModelWithDomain(domain), cancellationToken);
    }
}
