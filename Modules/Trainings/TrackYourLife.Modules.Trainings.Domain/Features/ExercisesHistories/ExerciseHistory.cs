using System.Text.Json;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

public sealed class ExerciseHistory : Entity<ExerciseHistoryId>, IAuditableEntity
{
    public OngoingTrainingId OngoingTrainingId { get; } = OngoingTrainingId.Empty;

    public ExerciseId ExerciseId { get; } = ExerciseId.Empty;

    public string NewExerciseSetsJson { get; private set; } = "[]";

    public IReadOnlyList<ExerciseSet> NewExerciseSets
    {
        get => JsonSerializer.Deserialize<List<ExerciseSet>>(NewExerciseSetsJson) ?? new();
        private set => NewExerciseSetsJson = JsonSerializer.Serialize(value.ToList());
    }

    public string OldExerciseSetsJson { get; private set; } = "[]";

    public IReadOnlyList<ExerciseSet> OldExerciseSets
    {
        get => JsonSerializer.Deserialize<List<ExerciseSet>>(OldExerciseSetsJson) ?? new();
        private set => OldExerciseSetsJson = JsonSerializer.Serialize(value.ToList());
    }

    public bool AreChangesApplied { get; private set; }

    public ExerciseStatus Status { get; private set; } = ExerciseStatus.Completed;

    public DateTime CreatedOnUtc { get; }

    public DateTime? ModifiedOnUtc { get; }

    private ExerciseHistory()
        : base() { }

    private ExerciseHistory(
        ExerciseHistoryId id,
        OngoingTrainingId ongoingTrainingId,
        ExerciseId exerciseId,
        List<ExerciseSet> oldExerciseSets,
        List<ExerciseSet> newExerciseSets,
        ExerciseStatus status,
        DateTime createdOnUtc
    )
        : base(id)
    {
        OngoingTrainingId = ongoingTrainingId;
        ExerciseId = exerciseId;
        CreatedOnUtc = createdOnUtc;
        OldExerciseSets = oldExerciseSets;
        NewExerciseSets = newExerciseSets;
        Status = status;
        AreChangesApplied = false;
    }

    public static Result<ExerciseHistory> Create(
        ExerciseHistoryId id,
        OngoingTrainingId ongoingTrainingId,
        ExerciseId exerciseId,
        List<ExerciseSet> oldExerciseSets,
        List<ExerciseSet> newExerciseSets,
        ExerciseStatus status,
        DateTime createdOnUtc
    )
    {
        var validationResult = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(ExerciseHistory), nameof(id))
            ),
            Ensure.NotEmptyId(
                ongoingTrainingId,
                DomainErrors.ArgumentError.Empty(nameof(ExerciseHistory), nameof(ongoingTrainingId))
            ),
            Ensure.NotEmptyId(
                exerciseId,
                DomainErrors.ArgumentError.Empty(nameof(ExerciseHistory), nameof(exerciseId))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(ExerciseHistory), nameof(createdOnUtc))
            )
        );

        if (validationResult.IsFailure)
        {
            return Result.Failure<ExerciseHistory>(validationResult.Error);
        }

        // For skipped exercises, allow empty sets. For completed exercises, require non-empty sets.
        if (status == ExerciseStatus.Completed)
        {
            var setsValidation = Result.FirstFailureOrSuccess(
                Ensure.NotEmptyList(
                    oldExerciseSets,
                    DomainErrors.ArgumentError.Empty(
                        nameof(ExerciseHistory),
                        nameof(oldExerciseSets)
                    )
                ),
                Ensure.NotEmptyList(
                    newExerciseSets,
                    DomainErrors.ArgumentError.Empty(
                        nameof(ExerciseHistory),
                        nameof(newExerciseSets)
                    )
                )
            );

            if (setsValidation.IsFailure)
            {
                return Result.Failure<ExerciseHistory>(setsValidation.Error);
            }
        }

        return Result.Success(
            new ExerciseHistory(
                id: id,
                ongoingTrainingId: ongoingTrainingId,
                exerciseId: exerciseId,
                oldExerciseSets: oldExerciseSets ?? new List<ExerciseSet>(),
                newExerciseSets: newExerciseSets ?? new List<ExerciseSet>(),
                status: status,
                createdOnUtc: createdOnUtc
            )
        );
    }

    public void SetAsChangesApplied()
    {
        AreChangesApplied = true;
    }

    public Result UpdateStatusAndSets(
        List<ExerciseSet> oldExerciseSets,
        List<ExerciseSet> newExerciseSets,
        ExerciseStatus status
    )
    {
        // For skipped exercises, allow empty sets. For completed exercises, require non-empty sets.
        if (status == ExerciseStatus.Completed)
        {
            var setsValidation = Result.FirstFailureOrSuccess(
                Ensure.NotEmptyList(
                    oldExerciseSets,
                    DomainErrors.ArgumentError.Empty(
                        nameof(ExerciseHistory),
                        nameof(oldExerciseSets)
                    )
                ),
                Ensure.NotEmptyList(
                    newExerciseSets,
                    DomainErrors.ArgumentError.Empty(
                        nameof(ExerciseHistory),
                        nameof(newExerciseSets)
                    )
                )
            );

            if (setsValidation.IsFailure)
            {
                return Result.Failure(setsValidation.Error);
            }
        }

        OldExerciseSets = oldExerciseSets;
        NewExerciseSets = newExerciseSets;
        Status = status;

        return Result.Success();
    }
}
