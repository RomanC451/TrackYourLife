using System.Linq.Expressions;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Common.Infrastructure.Data.Cookies.Specifications;

internal sealed class CookieReadModelWithDomainSpecification(string domain)
    : Specification<CookieReadModel, CookieId>
{
    public override Expression<Func<CookieReadModel, bool>> ToExpression() =>
        cookie => cookie.Domain == domain;
}
