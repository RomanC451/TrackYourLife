using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

public sealed class YoutubeSetting : Entity<YoutubeSettingsId>, IAuditableEntity
{
    public UserId UserId { get; } = UserId.Empty;
    public int MaxDivertissmentVideosPerDay { get; private set; }
    public SettingsChangeFrequency SettingsChangeFrequency { get; private set; }
    public int? DaysBetweenChanges { get; private set; }
    public DateTime? LastSettingsChangeUtc { get; private set; }
    public DayOfWeek? SpecificDayOfWeek { get; private set; }
    public int? SpecificDayOfMonth { get; private set; }
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; private set; }

    private YoutubeSetting()
        : base() { }

    private YoutubeSetting(
        YoutubeSettingsId id,
        UserId userId,
        int maxDivertissmentVideosPerDay,
        SettingsChangeFrequency settingsChangeFrequency,
        int? daysBetweenChanges,
        DateTime? lastSettingsChangeUtc,
        DayOfWeek? specificDayOfWeek,
        int? specificDayOfMonth,
        DateTime createdOnUtc
    )
        : base(id)
    {
        UserId = userId;
        MaxDivertissmentVideosPerDay = maxDivertissmentVideosPerDay;
        SettingsChangeFrequency = settingsChangeFrequency;
        DaysBetweenChanges = daysBetweenChanges;
        LastSettingsChangeUtc = lastSettingsChangeUtc;
        SpecificDayOfWeek = specificDayOfWeek;
        SpecificDayOfMonth = specificDayOfMonth;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<YoutubeSetting> Create(
        YoutubeSettingsId id,
        UserId userId,
        int maxDivertissmentVideosPerDay,
        SettingsChangeFrequency settingsChangeFrequency,
        int? daysBetweenChanges,
        DateTime? lastSettingsChangeUtc,
        DayOfWeek? specificDayOfWeek,
        int? specificDayOfMonth,
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

        // Validate frequency-specific fields
        var frequencyValidation = ValidateFrequencyFields(
            settingsChangeFrequency,
            daysBetweenChanges,
            specificDayOfWeek,
            specificDayOfMonth
        );

        if (frequencyValidation.IsFailure)
        {
            return Result.Failure<YoutubeSetting>(frequencyValidation.Error);
        }

        return Result.Success(
            new YoutubeSetting(
                id,
                userId,
                maxDivertissmentVideosPerDay,
                settingsChangeFrequency,
                daysBetweenChanges,
                lastSettingsChangeUtc,
                specificDayOfWeek,
                specificDayOfMonth,
                createdOnUtc
            )
        );
    }

    public Result UpdateSettings(
        int maxDivertissmentVideosPerDay,
        SettingsChangeFrequency settingsChangeFrequency,
        int? daysBetweenChanges,
        DayOfWeek? specificDayOfWeek,
        int? specificDayOfMonth,
        DateTime utcNow
    )
    {
        // Validate frequency-specific fields
        var frequencyValidation = ValidateFrequencyFields(
            settingsChangeFrequency,
            daysBetweenChanges,
            specificDayOfWeek,
            specificDayOfMonth
        );

        if (frequencyValidation.IsFailure)
        {
            return frequencyValidation;
        }

        MaxDivertissmentVideosPerDay = maxDivertissmentVideosPerDay;
        SettingsChangeFrequency = settingsChangeFrequency;
        DaysBetweenChanges = daysBetweenChanges;
        SpecificDayOfWeek = specificDayOfWeek;
        SpecificDayOfMonth = specificDayOfMonth;
        LastSettingsChangeUtc = utcNow;
        ModifiedOnUtc = utcNow;

        return Result.Success();
    }

    public Result CanChangeSettings(DateTime utcNow)
    {
        // If never changed before, allow change
        if (LastSettingsChangeUtc is null)
        {
            return Result.Success();
        }

        return SettingsChangeFrequency switch
        {
            SettingsChangeFrequency.OnceEveryFewDays => CanChangeBasedOnDays(utcNow),
            SettingsChangeFrequency.SpecificDayOfWeek => CanChangeBasedOnDayOfWeek(utcNow),
            SettingsChangeFrequency.SpecificDayOfMonth => CanChangeBasedOnDayOfMonth(utcNow),
            _ => Result.Failure(YoutubeSettingsErrors.InvalidFrequency),
        };
    }

    private Result CanChangeBasedOnDays(DateTime utcNow)
    {
        if (DaysBetweenChanges is null || LastSettingsChangeUtc is null)
        {
            return Result.Failure(YoutubeSettingsErrors.InvalidFrequencyConfiguration);
        }

        var nextAllowedChange = LastSettingsChangeUtc.Value.AddDays(DaysBetweenChanges.Value);

        if (utcNow < nextAllowedChange)
        {
            return Result.Failure(
                YoutubeSettingsErrors.SettingsChangeNotAllowed(nextAllowedChange)
            );
        }

        return Result.Success();
    }

    private Result CanChangeBasedOnDayOfWeek(DateTime utcNow)
    {
        if (SpecificDayOfWeek is null)
        {
            return Result.Failure(YoutubeSettingsErrors.InvalidFrequencyConfiguration);
        }

        if (utcNow.DayOfWeek != SpecificDayOfWeek.Value)
        {
            return Result.Failure(
                YoutubeSettingsErrors.SettingsChangeNotAllowedForDayOfWeek(SpecificDayOfWeek.Value)
            );
        }

        return Result.Success();
    }

    private Result CanChangeBasedOnDayOfMonth(DateTime utcNow)
    {
        if (SpecificDayOfMonth is null)
        {
            return Result.Failure(YoutubeSettingsErrors.InvalidFrequencyConfiguration);
        }

        if (utcNow.Day != SpecificDayOfMonth.Value)
        {
            return Result.Failure(
                YoutubeSettingsErrors.SettingsChangeNotAllowedForDayOfMonth(
                    SpecificDayOfMonth.Value
                )
            );
        }

        return Result.Success();
    }

    private static Result ValidateFrequencyFields(
        SettingsChangeFrequency frequency,
        int? daysBetweenChanges,
        DayOfWeek? specificDayOfWeek,
        int? specificDayOfMonth
    )
    {
        return frequency switch
        {
            SettingsChangeFrequency.OnceEveryFewDays => daysBetweenChanges.HasValue
            && daysBetweenChanges.Value > 0
                ? Result.Success()
                : Result.Failure(YoutubeSettingsErrors.InvalidDaysBetweenChanges),
            SettingsChangeFrequency.SpecificDayOfWeek => specificDayOfWeek.HasValue
                ? Result.Success()
                : Result.Failure(YoutubeSettingsErrors.SpecificDayOfWeekRequired),
            SettingsChangeFrequency.SpecificDayOfMonth => specificDayOfMonth.HasValue
            && specificDayOfMonth.Value >= 1
            && specificDayOfMonth.Value <= 31
                ? Result.Success()
                : Result.Failure(YoutubeSettingsErrors.InvalidDayOfMonth),
            _ => Result.Failure(YoutubeSettingsErrors.InvalidFrequency),
        };
    }
}
