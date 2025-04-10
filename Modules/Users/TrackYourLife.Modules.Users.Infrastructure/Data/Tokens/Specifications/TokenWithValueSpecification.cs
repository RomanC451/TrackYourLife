using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Tokens.Specifications;

internal sealed class TokenWithValueSpecification(string value) : Specification<Token, TokenId>
{
    public override Expression<Func<Token, bool>> ToExpression() => token => token.Value == value;
}

internal sealed class TokenReadModelWithValueSpecification(string value)
    : Specification<TokenReadModel, TokenId>
{
    public override Expression<Func<TokenReadModel, bool>> ToExpression() =>
        token => token.Value == value;
}
