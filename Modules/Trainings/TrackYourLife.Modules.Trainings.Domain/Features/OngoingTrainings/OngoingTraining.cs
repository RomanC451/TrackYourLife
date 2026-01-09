using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings.Events;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

public sealed class OngoingTraining : AggregateRoot<OngoingTrainingId>
{
    public UserId UserId { get; } = UserId.Empty;
    public Training Training { get; } = null!;
    public int ExerciseIndex { get; private set; }
    public int SetIndex { get; private set; }
    public DateTime StartedOnUtc { get; }
    public DateTime? FinishedOnUtc { get; private set; }

    public int ExercisesCount => Training.TrainingExercises.Count;

    public Exercise CurrentExercise
    {
        get
        {
            var orderedExercises = Training.TrainingExercises.OrderBy(e => e.OrderIndex).ToList();
            if (ExerciseIndex >= 0 && ExerciseIndex < orderedExercises.Count)
            {
                return orderedExercises[ExerciseIndex].Exercise;
            }
            throw new InvalidOperationException(
                $"Exercise index {ExerciseIndex} is out of bounds. Available exercises: {orderedExercises.Count}"
            );
        }
    }

    public int SetsCount => CurrentExercise.ExerciseSets.Count;

    public bool IsFinished => FinishedOnUtc.HasValue;
    public bool IsLastSet => SetIndex == SetsCount - 1;
    public bool IsLastExercise => ExerciseIndex == ExercisesCount - 1;
    public bool IsLastSetAndExercise => IsLastSet && IsLastExercise;

    public bool IsFirstSet => SetIndex == 0;
    public bool IsFirstExercise => ExerciseIndex == 0;
    public bool IsFirstSetAndExercise => IsFirstSet && IsFirstExercise;

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

        RaiseDirectDomainEvent(new OngoingTrainingFinishedDomainEvent(Id));

        return Result.Success();
    }

    public Result Next(IReadOnlySet<ExerciseId> completedOrSkippedExerciseIds)
    {
        if (IsFinished)
        {
            return Result.Failure(OngoingTrainingErrors.AlreadyFinished(Id));
        }

        // If not on the last set, just move to the next set
        if (!IsLastSet)
        {
            SetIndex++;
            return Result.Success();
        }

        // If on the last set, find the next uncompleted/unskipped exercise
        var orderedExercises = Training.TrainingExercises.OrderBy(e => e.OrderIndex).ToList();

        // First, try to find the next incomplete exercise after the current one
        var nextIncompleteExercise = orderedExercises
            .Skip(ExerciseIndex + 1)
            .FirstOrDefault(e => !completedOrSkippedExerciseIds.Contains(e.ExerciseId));

        int nextExerciseIndex;

        if (nextIncompleteExercise is not null)
        {
            // Found an incomplete exercise after the current one
            nextExerciseIndex = orderedExercises.IndexOf(nextIncompleteExercise);
        }
        else
        {
            // No incomplete exercises after current, wrap around to find the first incomplete exercise
            nextIncompleteExercise = orderedExercises
                .Take(ExerciseIndex + 1)
                .FirstOrDefault(e => !completedOrSkippedExerciseIds.Contains(e.ExerciseId));

            if (nextIncompleteExercise is null)
            {
                // All exercises are completed or skipped
                return Result.Failure(OngoingTrainingErrors.AllExercisesCompletedOrSkipped(Id));
            }

            nextExerciseIndex = orderedExercises.IndexOf(nextIncompleteExercise);
        }

        if (nextExerciseIndex < 0)
        {
            return Result.Failure(
                OngoingTrainingErrors.InvalidExerciseIndex(Id, nextExerciseIndex)
            );
        }

        // Move to the next incomplete exercise, first set
        ExerciseIndex = nextExerciseIndex;
        SetIndex = 0;

        return Result.Success();
    }

    public Result Previous()
    {
        if (IsFinished)
        {
            return Result.Failure(OngoingTrainingErrors.AlreadyFinished(Id));
        }

        if (IsFirstSetAndExercise)
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

    public Result SkipExercise(IReadOnlySet<ExerciseId> completedOrSkippedExerciseIds)
    {
        if (IsFinished)
        {
            return Result.Failure(OngoingTrainingErrors.AlreadyFinished(Id));
        }

        var orderedExercises = Training.TrainingExercises.OrderBy(e => e.OrderIndex).ToList();

        // First, try to find the next incomplete exercise after the current one
        var nextIncompleteExercise = orderedExercises
            .Skip(ExerciseIndex + 1)
            .FirstOrDefault(e => !completedOrSkippedExerciseIds.Contains(e.ExerciseId));

        int nextExerciseIndex;

        if (nextIncompleteExercise is not null)
        {
            // Found an incomplete exercise after the current one
            nextExerciseIndex = orderedExercises.IndexOf(nextIncompleteExercise);
        }
        else
        {
            // No incomplete exercises after current, wrap around to find the first incomplete exercise
            nextIncompleteExercise = orderedExercises
                .Take(ExerciseIndex + 1)
                .FirstOrDefault(e => !completedOrSkippedExerciseIds.Contains(e.ExerciseId));

            if (nextIncompleteExercise is null)
            {
                // All exercises are completed or skipped, stay on current exercise
                return Result.Success();
            }

            nextExerciseIndex = orderedExercises.IndexOf(nextIncompleteExercise);
        }

        if (nextExerciseIndex < 0)
        {
            return Result.Failure(
                OngoingTrainingErrors.InvalidExerciseIndex(Id, nextExerciseIndex)
            );
        }

        // Move to the next incomplete exercise, first set
        ExerciseIndex = nextExerciseIndex;
        SetIndex = 0;

        return Result.Success();
    }

    public Result JumpToExercise(int exerciseIndex)
    {
        if (IsFinished)
        {
            return Result.Failure(OngoingTrainingErrors.AlreadyFinished(Id));
        }

        var orderedExercises = Training.TrainingExercises.OrderBy(e => e.OrderIndex).ToList();

        if (exerciseIndex < 0 || exerciseIndex >= orderedExercises.Count)
        {
            return Result.Failure(OngoingTrainingErrors.InvalidExerciseIndex(Id, exerciseIndex));
        }

        ExerciseIndex = exerciseIndex;
        SetIndex = 0;

        return Result.Success();
    }

    public IReadOnlyList<ExerciseId> GetAllExerciseIds()
    {
        return Training
            .TrainingExercises.OrderBy(e => e.OrderIndex)
            .Select(te => te.Exercise.Id)
            .ToList();
    }
}
