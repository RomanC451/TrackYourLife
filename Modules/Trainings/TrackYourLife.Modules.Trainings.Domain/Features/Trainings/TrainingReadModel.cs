using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

public sealed record TrainingReadModel(
    TrainingId Id,
    UserId UserId,
    string Name,
    List<string> MuscleGroups,
    Difficulty Difficulty,
    string? Description,
    DateTime CreatedOnUtc,
    int Duration,
    int RestSeconds,
    DateTime? ModifiedOnUtc
) : IReadModel<TrainingId>
{
    public ICollection<TrainingExerciseReadModel> TrainingExercises { get; init; } = [];
}
