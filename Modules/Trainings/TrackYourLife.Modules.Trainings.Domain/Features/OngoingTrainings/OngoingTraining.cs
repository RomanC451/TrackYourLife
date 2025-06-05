using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

public sealed class OngoingTraining : Entity<OngoingTrainingId>
{
    public UserId UserId { get; } = UserId.Empty;
    public Training Training { get; } = null!;
    public int ExerciseIndex { get; private set; }
    public int SetIndex { get; private set; }
    public DateTime StartedOnUtc { get; }
    public DateTime? FinishedOnUtc { get; private set; }

    public int ExercisesCount => Training.TrainingExercises.Count;

    public Exercise CurrentExercise =>
        Training.TrainingExercises.OrderBy(e => e.OrderIndex).ElementAt(ExerciseIndex).Exercise;

    public int SetsCount => CurrentExercise.ExerciseSets.Count;

    public bool IsFinished => FinishedOnUtc.HasValue;
    public bool IsLastSet => SetIndex == SetsCount - 1;
    public bool IsLastExercise => ExerciseIndex == ExercisesCount - 1;
    public bool IsLastSetAndExercise => IsLastSet && IsLastExercise;
    public bool HasNext => !IsLastSetAndExercise;

    public bool IsFirstSet => SetIndex == 0;
    public bool IsFirstExercise => ExerciseIndex == 0;
    public bool IsFirstSetAndExercise => IsFirstSet && IsFirstExercise;
    public bool HasPrevious => !IsFirstSetAndExercise;

    private OngoingTraining()
        : base() { }

    private OngoingTraining(
        OngoingTrainingId id,
        UserId userId,
        Training training,
        int exerciseIndex,
        int setIndex,
        DateTime startedOnUtc
    )
        : base(id)
    {
        UserId = userId;
        Training = training;
        ExerciseIndex = exerciseIndex;
        SetIndex = setIndex;
        StartedOnUtc = startedOnUtc;
    }

    public static Result<OngoingTraining> Create(
        OngoingTrainingId id,
        UserId userId,
        Training training,
        DateTime startedOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(OngoingTraining), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(OngoingTraining), nameof(userId))
            ),
            Ensure.NotNull(
                training,
                DomainErrors.ArgumentError.Null(nameof(OngoingTraining), nameof(training))
            ),
            Ensure.NotEmpty(
                startedOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(OngoingTraining), nameof(startedOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<OngoingTraining>(result.Error);
        }

        var ongoingTraining = new OngoingTraining(
            id,
            userId,
            training,
            exerciseIndex: 0,
            setIndex: 0,
            startedOnUtc
        );
        return Result.Success(ongoingTraining);
    }

    public Result Finish(DateTime finishedOnUtc)
    {
        var result = Ensure.NotEmpty(
            finishedOnUtc,
            DomainErrors.ArgumentError.Empty(nameof(OngoingTraining), nameof(finishedOnUtc))
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        if (FinishedOnUtc.HasValue)
        {
            return Result.Failure(OngoingTrainingErrors.AlreadyFinished(Id));
        }

        FinishedOnUtc = finishedOnUtc;

        return Result.Success();
    }

    public Result Next()
    {
        if (IsFinished)
        {
            return Result.Failure(OngoingTrainingErrors.AlreadyFinished(Id));
        }

        if (!HasNext)
        {
            return Result.Failure(OngoingTrainingErrors.NoNext(Id));
        }

        if (IsLastSet)
        {
            ExerciseIndex++;
            SetIndex = 0;

            return Result.Success();
        }

        SetIndex++;

        return Result.Success();
    }

    public Result Previous()
    {
        if (IsFinished)
        {
            return Result.Failure(OngoingTrainingErrors.AlreadyFinished(Id));
        }

        if (!HasPrevious)
        {
            return Result.Failure(OngoingTrainingErrors.NoPrevious(Id));
        }

        if (IsFirstSet)
        {
            ExerciseIndex--;
            SetIndex = CurrentExercise.ExerciseSets.Count - 1;

            return Result.Success();
        }

        SetIndex--;

        return Result.Success();
    }
}
