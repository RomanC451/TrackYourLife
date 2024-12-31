using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Tokens.Specifications;

internal class TokenWithUserIdSpecification(UserId userId) : Specification<Token, TokenId>
{
    public override Expression<Func<Token, bool>> ToExpression() => token => token.UserId == userId;
}

internal class TokenReadModelWithUserIdSpecification(UserId userId)
    : Specification<TokenReadModel, TokenId>
{
    public override Expression<Func<TokenReadModel, bool>> ToExpression() =>
        token => token.UserId == userId;
}
