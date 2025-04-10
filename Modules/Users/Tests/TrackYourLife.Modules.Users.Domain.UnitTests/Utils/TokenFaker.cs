using Bogus;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.UnitTests.Utils;

public static class TokenFaker
{
    private static readonly Faker f = new();

    public static Token Generate(
        TokenId? id = null,
        string? value = null,
        UserId? userId = null,
        TokenType? type = null,
        DateTime? expiresAt = null,
        DeviceId? deviceId = null
    )
    {
        return Token
            .Create(
                id ?? TokenId.NewId(),
                value ?? f.Random.AlphaNumeric(32),
                userId ?? UserId.NewId(),
                type ?? f.PickRandom<TokenType>(),
                expiresAt ?? DateTime.UtcNow.AddDays(1),
                deviceId
            )
            .Value;
    }

    public static TokenReadModel GenerateReadModel(
        TokenId? id = null,
        UserId? userId = null,
        string? value = null,
        TokenType? type = null,
        DateTime? createdOn = null,
        DateTime? expiresAt = null,
        DeviceId? deviceId = null
    )
    {
        return new TokenReadModel(
            id ?? TokenId.NewId(),
            userId ?? UserId.NewId(),
            value ?? f.Random.AlphaNumeric(32),
            type ?? f.PickRandom<TokenType>(),
            createdOn ?? DateTime.UtcNow,
            expiresAt ?? DateTime.UtcNow.AddDays(1),
            deviceId ?? DeviceId.NewId()
        );
    }
}
