namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

public sealed class ExerciseSetChange
{
    public Guid SetId { get; }
    public float WeightChange { get; }
    public int RepsChange { get; }
}
