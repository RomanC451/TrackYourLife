using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

public sealed class Training : AggregateRoot<TrainingId>, IAuditableEntity
{
    public UserId UserId { get; } = UserId.Empty;
    public string Name { get; private set; } = string.Empty;
    public List<string> MuscleGroups { get; private set; } = [];
    public Difficulty Difficulty { get; private set; } = Difficulty.Easy;
    public int? Duration { get; private set; }
    public string? Description { get; private set; } = string.Empty;
    public int RestSeconds { get; private set; }
    public ICollection<TrainingExercise> TrainingExercises { get; private set; } = [];
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; }

    private Training()
        : base() { }

    private Training(
        TrainingId id,
        UserId userId,
        string name,
        List<string> muscleGroups,
        Difficulty difficulty,
        ICollection<TrainingExercise> trainingExercises,
        DateTime createdOnUtc,
        int duration,
        int restSeconds,
        string? description
    )
        : base(id)
    {
        UserId = userId;
        Name = name;
        MuscleGroups = muscleGroups;
        Difficulty = difficulty;
        Duration = duration;
        Description = description;
        RestSeconds = restSeconds;
        TrainingExercises = trainingExercises;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<Training> Create(
        TrainingId id,
        UserId userId,
        string name,
        List<string> muscleGroups,
        Difficulty difficulty,
        ICollection<TrainingExercise> trainingExercises,
        DateTime createdOnUtc,
        int duration,
        int restSeconds,
        string? description
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(Training), nameof(id))),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(userId))
            ),
            Ensure.NotEmpty(name, DomainErrors.ArgumentError.Empty(nameof(Training), nameof(name))),
            Ensure.NotNull(
                trainingExercises,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(trainingExercises))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(createdOnUtc))
            ),
            Ensure.NotNegative(
                duration,
                DomainErrors.ArgumentError.Invalid(nameof(Training), nameof(duration))
            ),
            Ensure.NotNegative(
                restSeconds,
                DomainErrors.ArgumentError.Invalid(nameof(Training), nameof(restSeconds))
            ),
            Ensure.NotEmptyList(
                muscleGroups,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(muscleGroups))
            ),
            Ensure.NotNull(
                difficulty,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(difficulty))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<Training>(result.Error);
        }

        return Result.Success(
            new Training(
                id: id,
                userId: userId,
                name: name,
                muscleGroups: muscleGroups,
                difficulty: difficulty,
                trainingExercises: trainingExercises,
                createdOnUtc: createdOnUtc,
                duration: duration,
                restSeconds: restSeconds,
                description: description
            )
        );
    }

    public Result UpdateDetails(
        string name,
        List<string> muscleGroups,
        Difficulty difficulty,
        int duration,
        int restSeconds,
        string? description
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(name, DomainErrors.ArgumentError.Empty(nameof(Training), nameof(name))),
            Ensure.NotNegative(
                duration,
                DomainErrors.ArgumentError.Invalid(nameof(Training), nameof(duration))
            ),
            Ensure.NotNegative(
                restSeconds,
                DomainErrors.ArgumentError.Invalid(nameof(Training), nameof(restSeconds))
            ),
            Ensure.NotEmptyList(
                muscleGroups,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(muscleGroups))
            ),
            Ensure.NotNull(
                difficulty,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(difficulty))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        Name = name;
        MuscleGroups = muscleGroups;
        Difficulty = difficulty;
        Duration = duration;
        Description = description;
        RestSeconds = restSeconds;

        return Result.Success();
    }

    public Result UpdateExercises(ICollection<TrainingExercise> trainingExercises)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyList(
                trainingExercises,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(trainingExercises))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        TrainingExercises = trainingExercises;

        return Result.Success();
    }

    public Result RemoveExercise(ExerciseId exerciseId)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                exerciseId,
                DomainErrors.ArgumentError.Empty(nameof(Training), nameof(exerciseId))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        TrainingExercises = TrainingExercises.Where(e => e.Exercise.Id != exerciseId).ToList();
        return Result.Success();
    }
}
