using System.Linq.Expressions;
using TrackYourLife.Common.Persistence.Specifications;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Specifications;

internal class UserWithEmailSpecification : Specification<User, UserId>
{
    private readonly Email _email;

    public UserWithEmailSpecification(Email email) => _email = email;

    public override Expression<Func<User, bool>> ToExpression() => User => User.Email == _email;
}
