using System.Linq.Expressions;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Tokens.Specifications;

internal sealed class TokenWithUserIdAndDeviceIdSpecification(UserId userId, DeviceId deviceId)
    : Specification<Token, TokenId>
{
    public override Expression<Func<Token, bool>> ToExpression() =>
        token => token.UserId == userId && token.DeviceId == deviceId;
}

internal sealed class TokenReadModelWithUserIdAndDeviceIdSpecification(
    UserId userId,
    DeviceId deviceId
) : Specification<TokenReadModel, TokenId>
{
    public override Expression<Func<TokenReadModel, bool>> ToExpression() =>
        token => token.UserId == userId && token.DeviceId == deviceId;
}
