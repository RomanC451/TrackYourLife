using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

public class ExerciseSet
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int? RestTimeSeconds { get; set; }

    public ExerciseSetType Type { get; set; }

    // Weight-based properties
    public int? Reps { get; set; }
    public float? Weight { get; set; }

    // Time-based properties
    public int? DurationSeconds { get; set; }

    // Distance-based properties
    public float? Distance { get; set; }

    [JsonConstructor]
    public ExerciseSet() { }

    protected ExerciseSet(
        Guid id,
        string name,
        int orderIndex,
        int? restTimeSeconds = null,
        int? reps = null,
        float? weight = null,
        int? durationSeconds = null,
        float? distance = null
    )
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        RestTimeSeconds = restTimeSeconds;
        Reps = reps;
        Weight = weight;
        DurationSeconds = durationSeconds;
        Distance = distance;
    }

    public static Result<ExerciseSet> CreateWeightBasedExerciseSet(
        Guid id,
        string name,
        int orderIndex,
        int reps,
        float weight,
        int? restTimeSeconds = null
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(id, DomainErrors.ArgumentError.Empty(nameof(ExerciseSet), nameof(Id))),
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(ExerciseSet), nameof(Name))
            ),
            Ensure.NotNegative(
                orderIndex,
                DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(OrderIndex))
            ),
            Ensure.NotNegative(
                reps,
                DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Reps))
            ),
            Ensure.NotNegative(
                weight,
                DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Weight))
            )
        );
        if (result.IsFailure)
        {
            return Result.Failure<ExerciseSet>(result.Error);
        }

        var exerciseSet = new ExerciseSet(
            id: id,
            name: name,
            orderIndex: orderIndex,
            reps: reps,
            weight: weight,
            restTimeSeconds: restTimeSeconds
        );
        exerciseSet.Type = ExerciseSetType.Weight;
        return Result.Success(exerciseSet);
    }

    public static Result<ExerciseSet> CreateTimeBasedExerciseSet(
        Guid id,
        string name,
        int orderIndex,
        int durationSeconds,
        int? restTimeSeconds = null
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotNegative(
                durationSeconds,
                DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(DurationSeconds))
            )
        );
        if (result.IsFailure)
        {
            return Result.Failure<ExerciseSet>(result.Error);
        }
        var exerciseSet = new ExerciseSet(
            id: id,
            name: name,
            orderIndex: orderIndex,
            restTimeSeconds: restTimeSeconds,
            durationSeconds: durationSeconds
        );
        exerciseSet.Type = ExerciseSetType.Time;
        return Result.Success(exerciseSet);
    }

    public static Result<ExerciseSet> CreateDistanceBasedExerciseSet(
        Guid id,
        string name,
        int orderIndex,
        float distance,
        int? restTimeSeconds = null
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotNegative(
                distance,
                DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Distance))
            )
        );
        if (result.IsFailure)
        {
            return Result.Failure<ExerciseSet>(result.Error);
        }
        var exerciseSet = new ExerciseSet(
            id: id,
            name: name,
            orderIndex: orderIndex,
            restTimeSeconds: restTimeSeconds,
            distance: distance
        );
        exerciseSet.Type = ExerciseSetType.Distance;
        return Result.Success(exerciseSet);
    }

    public static Result<ExerciseSet> CreateBodyweightExerciseSet(
        Guid id,
        string name,
        int orderIndex,
        int reps,
        int? restTimeSeconds = null
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(id, DomainErrors.ArgumentError.Empty(nameof(ExerciseSet), nameof(Id))),
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(ExerciseSet), nameof(Name))
            ),
            Ensure.NotNegative(
                orderIndex,
                DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(OrderIndex))
            ),
            Ensure.NotNegative(
                reps,
                DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Reps))
            )
        );
        if (result.IsFailure)
        {
            return Result.Failure<ExerciseSet>(result.Error);
        }

        var exerciseSet = new ExerciseSet(
            id: id,
            name: name,
            orderIndex: orderIndex,
            restTimeSeconds: restTimeSeconds,
            reps: reps
        );
        exerciseSet.Type = ExerciseSetType.Bodyweight;
        return Result.Success(exerciseSet);
    }

    public Result Update(
        int? reps = null,
        float? weight = null,
        int? durationSeconds = null,
        float? distance = null,
        string? customValue = null,
        string? customUnit = null,
        int? restTimeSeconds = null
    )
    {
        if (Type == ExerciseSetType.Weight)
        {
            var result = Result.FirstFailureOrSuccess(
                Ensure.NotNull(
                    reps,
                    DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Reps))
                ),
                Ensure.NotNull(
                    weight,
                    DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Weight))
                ),
                Ensure.NotNegative(
                    reps ?? 0,
                    DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Reps))
                ),
                Ensure.NotNegative(
                    weight ?? 0,
                    DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Weight))
                ),
                Ensure.NotNegative(
                    restTimeSeconds ?? 0,
                    DomainErrors.ArgumentError.Negative(
                        nameof(ExerciseSet),
                        nameof(RestTimeSeconds)
                    )
                )
            );
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            Reps = reps ?? 0;
            Weight = weight ?? 0;
            RestTimeSeconds = restTimeSeconds ?? 0;
        }
        else if (Type == ExerciseSetType.Time)
        {
            var result = Result.FirstFailureOrSuccess(
                Ensure.NotNull(
                    durationSeconds,
                    DomainErrors.ArgumentError.Negative(
                        nameof(ExerciseSet),
                        nameof(DurationSeconds)
                    )
                ),
                Ensure.NotNegative(
                    durationSeconds ?? 0,
                    DomainErrors.ArgumentError.Negative(
                        nameof(ExerciseSet),
                        nameof(DurationSeconds)
                    )
                ),
                Ensure.NotNegative(
                    restTimeSeconds ?? 0,
                    DomainErrors.ArgumentError.Negative(
                        nameof(ExerciseSet),
                        nameof(RestTimeSeconds)
                    )
                )
            );
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            DurationSeconds = durationSeconds ?? 0;
            RestTimeSeconds = restTimeSeconds ?? 0;
        }
        else if (Type == ExerciseSetType.Distance)
        {
            var result = Result.FirstFailureOrSuccess(
                Ensure.NotNull(
                    distance,
                    DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Distance))
                ),
                Ensure.NotNegative(
                    distance ?? 0,
                    DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Distance))
                ),
                Ensure.NotNegative(
                    restTimeSeconds ?? 0,
                    DomainErrors.ArgumentError.Negative(
                        nameof(ExerciseSet),
                        nameof(RestTimeSeconds)
                    )
                )
            );
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            Distance = distance ?? 0;
            RestTimeSeconds = restTimeSeconds ?? 0;
        }
        else
        {
            return Result.Failure(
                DomainErrors.ArgumentError.Invalid(nameof(ExerciseSet), nameof(Type))
            );
        }

        return Result.Success();
    }
}
