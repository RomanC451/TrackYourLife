namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

public sealed class ExerciseSetChange
{
    public Guid SetId { get; set; }
    public float WeightChange { get; set; }
    public int RepsChange { get; set; }
}
