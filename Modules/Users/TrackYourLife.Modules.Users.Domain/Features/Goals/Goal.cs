using TrackYourLife.Modules.Users.Domain.Features.Goals.Events;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Users.Domain.Features.Goals;

public sealed class Goal : AggregateRoot<GoalId>
{
    public UserId UserId { get; } = UserId.Empty;

    public GoalType Type { get; private set; }

    public int Value { get; private set; }

    public GoalPeriod Period { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    private Goal(
        GoalId id,
        UserId userId,
        GoalType type,
        int value,
        GoalPeriod perPeriod,
        DateOnly startDate,
        DateOnly endDate
    )
        : base(id)
    {
        UserId = userId;
        Type = type;
        Value = value;
        Period = perPeriod;
        StartDate = startDate;
        EndDate = endDate;
    }

    private Goal() { }

    public static Result<Goal> Create(
        GoalId id,
        UserId userId,
        GoalType type,
        int value,
        GoalPeriod perPeriod,
        DateOnly startDate,
        DateOnly? endDate = null
    )
    {
        endDate ??= DateOnly.MaxValue;

        Result result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(Goal), nameof(id))),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(Goal), nameof(userId))
            ),
            Ensure.NotEmpty(value, DomainErrors.ArgumentError.Empty(nameof(Goal), nameof(value))),
            Ensure.NotEmpty(
                startDate,
                DomainErrors.ArgumentError.Empty(nameof(Goal), nameof(startDate))
            ),
            Ensure.NotEmpty(
                endDate,
                DomainErrors.ArgumentError.Empty(nameof(Goal), nameof(endDate))
            ),
            Ensure.IsTrue(
                startDate <= endDate,
                DomainErrors.ArgumentError.Custom(
                    nameof(Goal),
                    nameof(startDate),
                    "EndDate must be greater or equal that StartDate."
                )
            )
        );

        if (result.IsFailure)
            return Result.Failure<Goal>(result.Error);

        Goal userGoal = new(id, userId, type, value, perPeriod, startDate, endDate.Value);

        userGoal.RaiseOutboxDomainEvent(new GoalCreatedDomainEvent(userGoal.UserId, userGoal.Id));

        return Result.Success(userGoal);
    }

    public Result UpdateType(GoalType type)
    {
        var result = Ensure.IsInEnum(
            type,
            DomainErrors.ArgumentError.Invalid(nameof(Goal), nameof(type))
        );

        if (result.IsFailure)
            return result;

        Type = type;

        return Result.Success();
    }

    public Result UpdateValue(int value)
    {
        var result = Ensure.NotEmpty(
            value,
            DomainErrors.ArgumentError.Empty(nameof(Goal), nameof(value))
        );

        if (result.IsFailure)
            return result;

        Value = value;

        return Result.Success();
    }

    public Result UpdatePerPeriod(GoalPeriod perPeriod)
    {
        var result = Ensure.IsInEnum(
            perPeriod,
            DomainErrors.ArgumentError.Invalid(nameof(Goal), nameof(perPeriod))
        );

        if (result.IsFailure)
            return result;
        Period = perPeriod;

        return Result.Success();
    }

    public Result UpdateStartDate(DateOnly startDate)
    {
        Result result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(
                startDate,
                DomainErrors.ArgumentError.Empty(nameof(Goal), nameof(startDate))
            ),
            Ensure.IsTrue(
                startDate <= EndDate,
                DomainErrors.ArgumentError.Custom(
                    nameof(Goal),
                    nameof(startDate),
                    "StartDate must be smaller or equal that EndDate."
                )
            )
        );

        if (result.IsFailure)
            return result;

        StartDate = startDate;

        return Result.Success();
    }

    public Result UpdateEndDate(DateOnly endDate)
    {
        Result result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(
                endDate,
                DomainErrors.ArgumentError.Empty(nameof(Goal), nameof(endDate))
            ),
            Ensure.IsTrue(
                endDate >= StartDate,
                DomainErrors.ArgumentError.Custom(
                    nameof(Goal),
                    nameof(endDate),
                    "EndDate must be greater that StartDate."
                )
            )
        );

        if (result.IsFailure)
            return result;

        EndDate = endDate;

        return Result.Success();
    }

    public bool IsActive() => EndDate == DateOnly.MaxValue;

    public bool FullyOverlappedBy(Goal otherGoal)
    {
        if (otherGoal.StartDate <= StartDate && otherGoal.EndDate >= EndDate)
            return true;

        return false;
    }

    public bool HasSameDates(Goal otherGoal) =>
        StartDate == otherGoal.StartDate && EndDate == otherGoal.EndDate;
}
