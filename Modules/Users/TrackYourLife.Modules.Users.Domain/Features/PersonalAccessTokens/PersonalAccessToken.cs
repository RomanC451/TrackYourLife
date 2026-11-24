using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

public sealed class PersonalAccessToken : Entity<PersonalAccessTokenId>
{
    public const string TokenPrefixMarker = "tyl_";
    public const int VisiblePrefixLength = 8;
    public static readonly TimeSpan DefaultLifetime = TimeSpan.FromDays(365 * 10);

    public UserId UserId { get; private set; } = UserId.Empty;
    public string Name { get; private set; } = string.Empty;
    public string TokenPrefix { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? LastUsedOnUtc { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedOnUtc { get; private set; }

    public bool IsRevoked => RevokedOnUtc.HasValue;
    public bool IsExpired(DateTime utcNow) => ExpiresAt <= utcNow;
    public bool IsActive(DateTime utcNow) => !IsRevoked && !IsExpired(utcNow);

    private PersonalAccessToken() { }

    private PersonalAccessToken(
        PersonalAccessTokenId id,
        UserId userId,
        string name,
        string tokenPrefix,
        string tokenHash,
        DateTime createdOnUtc,
        DateTime expiresAt
    )
        : base(id)
    {
        UserId = userId;
        Name = name;
        TokenPrefix = tokenPrefix;
        TokenHash = tokenHash;
        CreatedOnUtc = createdOnUtc;
        ExpiresAt = expiresAt;
    }

    public static Result<PersonalAccessToken> Create(
        PersonalAccessTokenId id,
        UserId userId,
        string name,
        string tokenPrefix,
        string tokenHash,
        DateTime createdOnUtc,
        DateTime expiresAt
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(PersonalAccessToken), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(PersonalAccessToken), nameof(userId))
            ),
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(PersonalAccessToken), nameof(name))
            ),
            Ensure.NotEmpty(
                tokenPrefix,
                DomainErrors.ArgumentError.Empty(nameof(PersonalAccessToken), nameof(tokenPrefix))
            ),
            Ensure.NotEmpty(
                tokenHash,
                DomainErrors.ArgumentError.Empty(nameof(PersonalAccessToken), nameof(tokenHash))
            ),
            Ensure.IsTrue(
                expiresAt > createdOnUtc,
                DomainErrors.ArgumentError.Invalid(nameof(PersonalAccessToken), nameof(expiresAt))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<PersonalAccessToken>(result.Error);
        }

        return Result.Success(
            new PersonalAccessToken(id, userId, name.Trim(), tokenPrefix, tokenHash, createdOnUtc, expiresAt)
        );
    }

    public void MarkUsed(DateTime utcNow)
    {
        LastUsedOnUtc = utcNow;
    }

    public Result Revoke(DateTime utcNow)
    {
        if (IsRevoked)
        {
            return Result.Failure(PersonalAccessTokenErrors.AlreadyRevoked);
        }

        RevokedOnUtc = utcNow;
        return Result.Success();
    }
}
