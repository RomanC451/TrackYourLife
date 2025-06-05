namespace TrackYourLife.Modules.Trainings.Domain.Features.Sets;

public sealed class ExerciseSet
{
    public string Name { get; } = string.Empty;
    public int Reps { get; }
    public float Weight { get; }

    private ExerciseSet()
        : base() { }

    public ExerciseSet(string name, int reps, float weight)
    {
        Name = name;
        Reps = reps;
        Weight = weight;
    }
}
