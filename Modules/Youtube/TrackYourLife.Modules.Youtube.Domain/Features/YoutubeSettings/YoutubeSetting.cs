using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

public sealed class YoutubeSetting : Entity<YoutubeSettingsId>, IAuditableEntity
{
    public UserId UserId { get; } = UserId.Empty;
    public string? SettingsPasswordHash { get; private set; }
    public DateTime? SettingsPasswordResetEmailSentAtUtc { get; private set; }
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; private set; }

    public static readonly TimeSpan PasswordResetEmailCooldown = TimeSpan.FromMinutes(5);

    public bool HasPassword => !string.IsNullOrEmpty(SettingsPasswordHash);

    private YoutubeSetting()
        : base() { }

    private YoutubeSetting(
        YoutubeSettingsId id,
        UserId userId,
        string? settingsPasswordHash,
        DateTime? settingsPasswordResetEmailSentAtUtc,
        DateTime createdOnUtc
    )
        : base(id)
    {
        UserId = userId;
        SettingsPasswordHash = settingsPasswordHash;
        SettingsPasswordResetEmailSentAtUtc = settingsPasswordResetEmailSentAtUtc;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<YoutubeSetting> Create(
        YoutubeSettingsId id,
        UserId userId,
        string? settingsPasswordHash,
        DateTime createdOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeSetting), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeSetting), nameof(userId))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeSetting), nameof(createdOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<YoutubeSetting>(result.Error);
        }

        return Result.Success(
            new YoutubeSetting(id, userId, settingsPasswordHash, null, createdOnUtc)
        );
    }

    public Result CanRequestPasswordResetEmail(DateTime utcNow, TimeSpan cooldown)
    {
        if (!HasPassword)
        {
            return Result.Failure(YoutubeSettingsErrors.PasswordNotSet);
        }

        if (
            SettingsPasswordResetEmailSentAtUtc.HasValue
            && utcNow - SettingsPasswordResetEmailSentAtUtc.Value < cooldown
        )
        {
            return Result.Failure(YoutubeSettingsErrors.ResetEmailRateLimited);
        }

        return Result.Success();
    }

    public Result RecordPasswordResetEmailSent(DateTime utcNow)
    {
        SettingsPasswordResetEmailSentAtUtc = utcNow;
        ModifiedOnUtc = utcNow;

        return Result.Success();
    }

    public Result SetPasswordHash(string passwordHash, DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result.Failure(YoutubeSettingsErrors.NewPasswordRequired);
        }

        SettingsPasswordHash = passwordHash;
        ModifiedOnUtc = utcNow;

        return Result.Success();
    }

    public Result ClearPassword(DateTime utcNow)
    {
        SettingsPasswordHash = null;
        ModifiedOnUtc = utcNow;

        return Result.Success();
    }
}
