using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

public sealed record WorkoutPlanTrainingReadModel(
    WorkoutPlanId WorkoutPlanId,
    TrainingId TrainingId,
    int OrderIndex
)
{
    public TrainingReadModel Training { get; set; } = null!;
}
