using System.Linq.Expressions;
using TrackYourLife.Common.Persistence.Specifications;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Specifications;

internal class UserTokenWithValueSpecification : Specification<UserToken, UserTokenId>
{
    private readonly string _value;

    public UserTokenWithValueSpecification(string value) => _value = value;

    public override Expression<Func<UserToken, bool>> ToExpression() =>
        token => token.Value == _value;
}
