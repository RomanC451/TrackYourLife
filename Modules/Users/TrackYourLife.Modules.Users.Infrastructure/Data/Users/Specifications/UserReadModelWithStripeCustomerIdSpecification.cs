using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Users.Specifications;

internal sealed class UserReadModelWithStripeCustomerIdSpecification(string stripeCustomerId)
    : Specification<UserReadModel, UserId>
{
    public override Expression<Func<UserReadModel, bool>> ToExpression() =>
        user => user.StripeCustomerId == stripeCustomerId;
}
