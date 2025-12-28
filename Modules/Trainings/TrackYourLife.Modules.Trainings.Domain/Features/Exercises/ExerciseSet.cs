using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

public class ExerciseSet
{
    public Guid Id { get; private set; } = Guid.Empty;
    public string Name { get; private set; } = string.Empty;
    public int OrderIndex { get; private set; }
    public int? RestTimeSeconds { get; private set; }

    // Generic count/unit properties
    public float Count1 { get; private set; }
    public string Unit1 { get; private set; } = string.Empty;
    public float? Count2 { get; private set; }
    public string? Unit2 { get; private set; }

    // [JsonConstructor]
    protected ExerciseSet() { }

    [JsonConstructor]
    protected ExerciseSet(
        Guid id,
        string name,
        int orderIndex,
        float count1,
        string unit1,
        float? count2 = null,
        string? unit2 = null,
        int? restTimeSeconds = null
    )
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        Count1 = count1;
        Unit1 = unit1;
        Count2 = count2;
        Unit2 = unit2;
        RestTimeSeconds = restTimeSeconds;
    }

    public static Result<ExerciseSet> Create(
        Guid id,
        string name,
        int orderIndex,
        float count1,
        string unit1,
        float? count2 = null,
        string? unit2 = null,
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
                count1,
                DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Count1))
            ),
            Ensure.NotEmpty(
                unit1,
                DomainErrors.ArgumentError.Empty(nameof(ExerciseSet), nameof(Unit1))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<ExerciseSet>(result.Error);
        }

        // Validate that if Count2 is provided, Unit2 must be provided and vice versa
        if (count2.HasValue && string.IsNullOrEmpty(unit2))
        {
            return Result.Failure<ExerciseSet>(
                DomainErrors.ArgumentError.Empty(nameof(ExerciseSet), nameof(Unit2))
            );
        }

        if (!count2.HasValue && !string.IsNullOrEmpty(unit2))
        {
            return Result.Failure<ExerciseSet>(
                DomainErrors.ArgumentError.Invalid(nameof(ExerciseSet), nameof(Count2))
            );
        }

        if (count2.HasValue)
        {
            var count2Result = Ensure.NotNegative(
                count2.Value,
                DomainErrors.ArgumentError.Negative(nameof(ExerciseSet), nameof(Count2))
            );
            if (count2Result.IsFailure)
            {
                return Result.Failure<ExerciseSet>(count2Result.Error);
            }
        }

        var exerciseSet = new ExerciseSet(
            id: id,
            name: name,
            orderIndex: orderIndex,
            count1: count1,
            unit1: unit1,
            count2: count2,
            unit2: unit2,
            restTimeSeconds: restTimeSeconds
        );

        return Result.Success(exerciseSet);
    }
}
