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

    public string ExerciseSetChangesJson { get; private set; } = "[]";

    public IReadOnlyList<ExerciseSetChange> ExerciseSetChanges
    {
        get => JsonSerializer.Deserialize<List<ExerciseSetChange>>(ExerciseSetChangesJson) ?? [];
        private set => ExerciseSetChangesJson = JsonSerializer.Serialize(value);
    }

    public string ExerciseSetsBeforeChangeJson { get; private set; } = "[]";

    public IReadOnlyList<ExerciseSet> ExerciseSetsBeforeChange
    {
        get => JsonSerializer.Deserialize<List<ExerciseSet>>(ExerciseSetsBeforeChangeJson) ?? [];
        private set => ExerciseSetsBeforeChangeJson = JsonSerializer.Serialize(value);
    }

    public bool AreChangesApplied { get; private set; }

    public DateTime CreatedOnUtc { get; }

    public DateTime? ModifiedOnUtc { get; }

    private ExerciseHistory()
        : base() { }

    private ExerciseHistory(
        ExerciseHistoryId id,
        OngoingTrainingId ongoingTrainingId,
        ExerciseId exerciseId,
        List<ExerciseSet> exerciseSetsBeforeChange,
        List<ExerciseSetChange> exerciseSetChanges,
        DateTime createdOnUtc
    )
        : base(id)
    {
        OngoingTrainingId = ongoingTrainingId;
        ExerciseId = exerciseId;
        CreatedOnUtc = createdOnUtc;
        ExerciseSetsBeforeChange = exerciseSetsBeforeChange;
        ExerciseSetChanges = exerciseSetChanges;
        AreChangesApplied = false;
    }

    public static Result<ExerciseHistory> Create(
        ExerciseHistoryId id,
        OngoingTrainingId ongoingTrainingId,
        ExerciseId exerciseId,
        List<ExerciseSet> exerciseSets,
        List<ExerciseSetChange> exerciseSetChanges,
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
            ),
            Ensure.NotEmpty(
                exerciseSets,
                DomainErrors.ArgumentError.Empty(nameof(ExerciseHistory), nameof(exerciseSets))
            ),
            Ensure.NotEmpty(
                exerciseSetChanges,
                DomainErrors.ArgumentError.Empty(
                    nameof(ExerciseHistory),
                    nameof(exerciseSetChanges)
                )
            )
        );

        if (validationResult.IsFailure)
        {
            return Result.Failure<ExerciseHistory>(validationResult.Error);
        }

        return Result.Success(
            new ExerciseHistory(
                id: id,
                ongoingTrainingId: ongoingTrainingId,
                exerciseId: exerciseId,
                exerciseSetsBeforeChange: exerciseSets,
                exerciseSetChanges: exerciseSetChanges,
                createdOnUtc: createdOnUtc
            )
        );
    }

    public void SetAsChangesApplied()
    {
        AreChangesApplied = true;
    }
}
