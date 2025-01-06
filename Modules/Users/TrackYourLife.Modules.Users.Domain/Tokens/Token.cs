using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Users.Domain.Tokens;

public sealed class Token : Entity<TokenId>
{
    public TokenType Type { get; init; }

    public string Value { get; private set; } = string.Empty;

    public DateTime CreatedOn { get; init; }

    public DateTime ExpiresAt { get; private set; }
    public UserId UserId { get; init; } = UserId.Empty;

    public DeviceId? DeviceId { get; init; } = null;

    private Token(
        TokenId id,
        string value,
        UserId userId,
        TokenType type,
        DateTime expiresAt,
        DeviceId? deviceId = null
    )
        : base(id)
    {
        Value = value;
        UserId = userId;
        DeviceId = deviceId;
        CreatedOn = DateTime.UtcNow;
        Type = type;
        ExpiresAt = expiresAt;
    }

    private Token() { }

    public static Result<Token> Create(
        TokenId id,
        string value,
        UserId userId,
        TokenType type,
        DateTime expiresAt,
        DeviceId? deviceId = null
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Null(nameof(Token), nameof(userId))
            ),
            Ensure.NotEmpty(value, DomainErrors.ArgumentError.Empty(nameof(Token), nameof(value))),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Null(nameof(Token), nameof(userId))
            ),
            Ensure.IsInEnum(type, DomainErrors.ArgumentError.Invalid(nameof(Token), nameof(type))),
            Ensure.IsTrue(
                expiresAt > DateTime.UtcNow,
                DomainErrors.ArgumentError.Invalid(nameof(Token), nameof(expiresAt))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<Token>(error: result.Error);
        }

        var token = new Token(id, value, userId, type, expiresAt, deviceId);

        return Result.Success(token);
    }

    public Result UpdateValue(string newValue)
    {
        var result = Ensure.NotEmpty(
            newValue,
            DomainErrors.ArgumentError.Empty(nameof(Token), nameof(newValue))
        );

        if (result.IsFailure)
        {
            return Result.Failure(error: result.Error);
        }

        Value = newValue;

        return Result.Success();
    }

    public Result UpdateExpiresAt(DateTime dateTime)
    {
        var result = Ensure.IsTrue(
            dateTime > DateTime.UtcNow,
            DomainErrors.ArgumentError.Invalid(nameof(Token), nameof(dateTime))
        );

        if (result.IsFailure)
        {
            return Result.Failure(error: result.Error);
        }

        ExpiresAt = dateTime;

        return Result.Success();
    }
}
