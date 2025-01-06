using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Tokens.Specifications;

internal class TokenWithUserIdAndDeviceIdSpecification(UserId userId, DeviceId deviceId)
    : Specification<Token, TokenId>
{
    public override Expression<Func<Token, bool>> ToExpression() =>
        token => token.UserId == userId && token.DeviceId == deviceId;
}

internal class TokenReadModelWithUserIdAndDeviceIdSpecification(UserId userId, DeviceId deviceId)
    : Specification<TokenReadModel, TokenId>
{
    public override Expression<Func<TokenReadModel, bool>> ToExpression() =>
        token => token.UserId == userId && token.DeviceId == deviceId;
}
