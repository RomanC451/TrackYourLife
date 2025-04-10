using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Tokens.Specifications;

internal sealed class TokenWithUserIdAndTypeSpecification(UserId userId, TokenType tokenType)
    : Specification<Token, TokenId>
{
    public override Expression<Func<Token, bool>> ToExpression() =>
        token => token.UserId == userId && token.Type == tokenType;
}

internal sealed class TokenReadModelWithUserIdAndTypeSpecification(
    UserId userId,
    TokenType tokenType
) : Specification<TokenReadModel, TokenId>
{
    public override Expression<Func<TokenReadModel, bool>> ToExpression() =>
        token => token.UserId == userId && token.Type == tokenType;
}
