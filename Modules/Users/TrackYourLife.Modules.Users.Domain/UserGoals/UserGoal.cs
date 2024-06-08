using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Primitives;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users;
using TrackYourLife.Common.Domain.Utility;

namespace TrackYourLife.Modules.Users.Domain.UserGoals;

public class UserGoal : AggregateRoot<UserGoalId>
{
    private UserGoal() { }

    private UserGoal(
        UserGoalId id,
        UserId userId,
        UserGoalType type,
        int value,
        UserGoalPerPeriod perPeriod,
        DateOnly startDate,
        DateOnly endDate
    )
        : base(id)
    {
        UserId = userId;
        Type = type;
        Value = value;
        PerPeriod = perPeriod;
        StartDate = startDate;
        EndDate = endDate;
    }

    public UserId UserId { get; init; } = UserId.Empty;

    public UserGoalType Type { get; private set; }

    public int Value { get; private set; }

    public UserGoalPerPeriod PerPeriod { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public static Result<UserGoal> Create(
        UserGoalId id,
        UserId userId,
        UserGoalType type,
        int value,
        UserGoalPerPeriod perPeriod,
        DateOnly startDate,
        DateOnly? endDate = null
    )
    {
        endDate ??= DateOnly.MaxValue;

        Result result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(id))),
            Ensure.NotEmptyId(userId, DomainErrors.ArgumentError.Empty(nameof(userId))),
            Ensure.NotEmpty(value, DomainErrors.ArgumentError.Empty(nameof(value))),
            Ensure.NotEmpty(startDate, DomainErrors.ArgumentError.Empty(nameof(startDate))),
            Ensure.NotEmpty(endDate, DomainErrors.ArgumentError.Empty(nameof(endDate))),
            Ensure.IsTrue(
                startDate <= endDate,
                DomainErrors.ArgumentError.Custom(
                    nameof(startDate),
                    "EndDate must be greater or equal that StartDate."
                )
            )
        );

        if (result.IsFailure)
            return Result.Failure<UserGoal>(result.Error);

        UserGoal userGoal = new(id, userId, type, value, perPeriod, startDate, endDate.Value);

        userGoal.RaiseDomainEvent(
            new UserGoalCreatedDomainEvent(
                userGoal.UserId,
                userGoal.Id,
                userGoal.Type,
                userGoal.StartDate
            )
        );

        return Result.Success(userGoal);
    }

    public void UpdateType(UserGoalType type) => Type = type;

    public Result UpdateValue(int value)
    {
        var result = Ensure.NotEmpty(value, DomainErrors.ArgumentError.Empty(nameof(value)));

        if (result.IsFailure)
            return result;

        Value = value;

        return Result.Success();
    }

    public void UpdatePerPeriod(UserGoalPerPeriod perPeriod) => PerPeriod = perPeriod;

    public Result UpdateStartDate(DateOnly startDate)
    {
        Result result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(startDate, DomainErrors.ArgumentError.Empty(nameof(startDate))),
            Ensure.IsTrue(
                startDate <= EndDate,
                DomainErrors.ArgumentError.Custom(
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
            Ensure.NotEmpty(endDate, DomainErrors.ArgumentError.Empty(nameof(endDate))),
            Ensure.IsTrue(
                endDate >= StartDate,
                DomainErrors.ArgumentError.Custom(
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

    public bool FullyOverlappedBy(UserGoal otherGoal)
    {
        if (otherGoal.StartDate <= StartDate && otherGoal.EndDate >= EndDate)
            return true;

        return false;
    }
}
