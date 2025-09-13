using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

public sealed class ExerciseSet
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; } = string.Empty;
    public int Reps { get; private set; }
    public float Weight { get; private set; }
    public int OrderIndex { get; }

    private ExerciseSet()
        : base() { }

    [JsonConstructor]
    public ExerciseSet(Guid id, string name, int reps, float weight, int orderIndex)
    {
        Id = id;
        Name = name;
        Reps = reps;
        Weight = weight;
        OrderIndex = orderIndex;
    }

    public Result Update(int reps, float weight)
    {
        var result = Result.FirstFailureOrSuccess(
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
            return Result.Failure(result.Error);
        }

        Reps = reps;
        Weight = weight;
        return Result.Success();
    }
}
