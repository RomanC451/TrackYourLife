using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.Modules.Common.Infrastructure.Data.Cookies.Specifications;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Common.Infrastructure.Data.Cookies;

internal sealed class CookieRepository(CommonWriteDbContext dbContext)
    : GenericRepository<Cookie, CookieId>(dbContext.Cookies),
        ICookieRepository
{
    public Task<Cookie?> GetByNameAndDomainAsync(
        string name,
        string domain,
        CancellationToken cancellationToken
    )
    {
        return FirstOrDefaultAsync(
            new CookieWithNameAndDomainSpecification(name, domain),
            cancellationToken
        );
    }
}
