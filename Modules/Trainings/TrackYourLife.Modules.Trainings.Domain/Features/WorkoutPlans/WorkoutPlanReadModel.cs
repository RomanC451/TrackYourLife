using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

public sealed record WorkoutPlanReadModel(
    WorkoutPlanId Id,
    UserId UserId,
    string Name,
    bool IsActive,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<WorkoutPlanId>
{
    public ICollection<WorkoutPlanTrainingReadModel> WorkoutPlanTrainings { get; set; } = [];

    public IEnumerable<TrainingReadModel> GetOrderedTrainings() =>
        WorkoutPlanTrainings.OrderBy(wpt => wpt.OrderIndex).Select(wpt => wpt.Training);
}
