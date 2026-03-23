using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

public interface IWorkoutPlansQuery
{
    Task<WorkoutPlanReadModel?> GetByIdAsync(
        WorkoutPlanId id,
        CancellationToken cancellationToken = default
    );

    Task<WorkoutPlanReadModel?> GetActiveByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<WorkoutPlanReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );
}
