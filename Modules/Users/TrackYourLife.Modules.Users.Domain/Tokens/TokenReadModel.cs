using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Tokens;

public sealed record TokenReadModel(
    TokenId Id,
    UserId UserId,
    string Value,
    TokenType Type,
    DateTime CreatedOn,
    DateTime ExpiresAt,
    DeviceId DeviceId
) : IReadModel<TokenId>;
