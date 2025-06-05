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
    public int? Duration { get; private set; }
    public string? Description { get; private set; } = string.Empty;
    public ICollection<TrainingExercise> TrainingExercises { get; private set; } = [];
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; private set; } = null;

    private Training()
        : base() { }

    private Training(
        TrainingId id,
        UserId userId,
        string name,
        ICollection<TrainingExercise> trainingExercises,
        DateTime createdOnUtc,
        int duration,
        string? description
    )
        : base(id)
    {
        UserId = userId;
        Name = name;
        Duration = duration;
        Description = description;
        TrainingExercises = trainingExercises;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<Training> Create(
        TrainingId id,
        UserId userId,
        string name,
        ICollection<TrainingExercise> trainingExercises,
        DateTime createdOnUtc,
        int duration,
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
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<Training>(result.Error);
        }

        return Result.Success(
            new Training(id, userId, name, trainingExercises, createdOnUtc, duration, description)
        );
    }

    public Result UpdateDetails(string name, int duration, string? description, DateTime modifiedOn)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(name, DomainErrors.ArgumentError.Empty(nameof(Training), nameof(name))),
            Ensure.NotNegative(
                duration,
                DomainErrors.ArgumentError.Invalid(nameof(Training), nameof(duration))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        Name = name;
        Duration = duration;
        Description = description;

        ModifiedOnUtc = modifiedOn;

        return Result.Success();
    }

    public Result UpdateExercises(
        ICollection<TrainingExercise> trainingExercises,
        DateTime modifiedOn
    )
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
        ModifiedOnUtc = modifiedOn;

        return Result.Success();
    }

    public Result RemoveExercise(ExerciseId exerciseId, DateTime modifiedOn)
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
        ModifiedOnUtc = modifiedOn;
        return Result.Success();
    }
}
