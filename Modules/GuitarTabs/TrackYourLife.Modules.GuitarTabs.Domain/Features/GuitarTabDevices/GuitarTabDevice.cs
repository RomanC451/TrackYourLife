using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

public sealed class GuitarTabDevice : Entity<GuitarTabDeviceId>, IAuditableEntity
{
    public const int NameMaxLength = 80;
    public const decimal DefaultSpeed = 0.2m;
    public const decimal MinSpeed = 0.1m;
    public const decimal MaxSpeed = 20m;
    public const int MinSemitones = -11;
    public const int MaxSemitones = 11;

    public UserId UserId { get; } = UserId.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal Speed { get; private set; } = DefaultSpeed;
    public int Semitones { get; private set; }
    public bool UseFlats { get; private set; }
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; private set; }

    private GuitarTabDevice()
        : base() { }

    private GuitarTabDevice(
        GuitarTabDeviceId id,
        UserId userId,
        string name,
        decimal speed,
        int semitones,
        bool useFlats,
        DateTime createdOnUtc
    )
        : base(id)
    {
        UserId = userId;
        Name = name;
        Speed = speed;
        Semitones = semitones;
        UseFlats = useFlats;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<GuitarTabDevice> Create(
        GuitarTabDeviceId id,
        UserId userId,
        string name,
        decimal speed,
        int semitones,
        bool useFlats,
        DateTime createdOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(GuitarTabDevice), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(GuitarTabDevice), nameof(userId))
            ),
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(GuitarTabDevice), nameof(name))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(GuitarTabDevice), nameof(createdOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<GuitarTabDevice>(result.Error);
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > NameMaxLength)
        {
            return Result.Failure<GuitarTabDevice>(
                DomainErrors.ArgumentError.Invalid(nameof(GuitarTabDevice), nameof(name))
            );
        }

        var settingsResult = ValidateSettings(speed, semitones);
        if (settingsResult.IsFailure)
        {
            return Result.Failure<GuitarTabDevice>(settingsResult.Error);
        }

        return Result.Success(
            new GuitarTabDevice(id, userId, trimmedName, speed, semitones, useFlats, createdOnUtc)
        );
    }

    public Result UpdateSettings(
        decimal speed,
        int semitones,
        bool useFlats,
        DateTime modifiedOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            ValidateSettings(speed, semitones),
            Ensure.NotEmpty(
                modifiedOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(GuitarTabDevice), nameof(modifiedOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return result;
        }

        Speed = speed;
        Semitones = semitones;
        UseFlats = useFlats;
        ModifiedOnUtc = modifiedOnUtc;
        return Result.Success();
    }

    private static Result ValidateSettings(decimal speed, int semitones)
    {
        if (decimal.Round(speed, 1, MidpointRounding.AwayFromZero) != speed
            || speed < MinSpeed
            || speed > MaxSpeed)
        {
            return Result.Failure(
                DomainErrors.ArgumentError.Invalid(nameof(GuitarTabDevice), nameof(speed))
            );
        }

        if (semitones < MinSemitones || semitones > MaxSemitones)
        {
            return Result.Failure(
                DomainErrors.ArgumentError.Invalid(nameof(GuitarTabDevice), nameof(semitones))
            );
        }

        return Result.Success();
    }
}
