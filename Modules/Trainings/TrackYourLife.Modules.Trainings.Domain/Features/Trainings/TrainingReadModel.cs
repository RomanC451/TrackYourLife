using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

public sealed record TrainingReadModel(
    TrainingId Id,
    UserId UserId,
    string Name,
    string? Description,
    DateTime CreatedOnUtc,
    int Duration,
    DateTime? ModifiedOnUtc
) : IReadModel<TrainingId>
{
    public ICollection<TrainingExerciseReadModel> TrainingExercises { get; init; } = [];
}
