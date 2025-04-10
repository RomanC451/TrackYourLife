using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Users.Specifications;

internal sealed class UserWithEmailSpecification(Email email) : Specification<User, UserId>
{
    public override Expression<Func<User, bool>> ToExpression() => User => User.Email == email;
}

internal sealed class UserReadModelWithEmailSpecification(Email email)
    : Specification<UserReadModel, UserId>
{
    public override Expression<Func<UserReadModel, bool>> ToExpression() =>
        User => User.Email == email.Value;
}
