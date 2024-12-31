using System.Linq.Expressions;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Common.Infrastructure.Data.Cookies.Specifications;

public class CookieWithNameAndDomain(string name, string domain) : Specification<Cookie, CookieId>
{
    public override Expression<Func<Cookie, bool>> ToExpression() =>
        cookie => cookie.Name == name && cookie.Domain == domain;
}
