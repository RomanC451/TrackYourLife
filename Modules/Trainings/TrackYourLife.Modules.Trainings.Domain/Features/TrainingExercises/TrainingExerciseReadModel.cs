using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;

public sealed record TrainingExerciseReadModel(
    TrainingId TrainingId,
    ExerciseId ExerciseId,
    int OrderIndex
)
{
    public ExerciseReadModel Exercise { get; set; } = null!;
}
