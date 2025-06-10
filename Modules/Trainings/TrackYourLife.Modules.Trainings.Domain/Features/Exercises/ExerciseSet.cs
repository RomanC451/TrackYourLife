using System.Text.Json.Serialization;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

public sealed class ExerciseSet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; } = string.Empty;
    public int Reps { get; }
    public float Weight { get; }
    public int OrderIndex { get; }

    private ExerciseSet()
        : base() { }

    [JsonConstructor]
    public ExerciseSet(string name, int reps, float weight, int orderIndex)
    {
        Name = name;
        Reps = reps;
        Weight = weight;
        OrderIndex = orderIndex;
    }

    public ExerciseSet(Guid id, string name, int reps, float weight, int orderIndex)
    {
        Id = id;
        Name = name;
        Reps = reps;
        Weight = weight;
        OrderIndex = orderIndex;
    }
}
