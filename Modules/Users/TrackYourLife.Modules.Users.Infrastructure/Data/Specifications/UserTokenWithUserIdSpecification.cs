using System.Linq.Expressions;
using TrackYourLife.Common.Persistence.Specifications;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Specifications;

internal class UserTokenWithUserIdSpecification : Specification<UserToken, UserTokenId>
{
    private readonly UserId _userId;

    public UserTokenWithUserIdSpecification(UserId userId) => _userId = userId;

    public override Expression<Func<UserToken, bool>> ToExpression() =>
        token => token.UserId == _userId;
}
