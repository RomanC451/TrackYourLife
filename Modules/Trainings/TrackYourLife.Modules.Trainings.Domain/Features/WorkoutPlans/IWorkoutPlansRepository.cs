using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

public interface IWorkoutPlansRepository
{
    Task<WorkoutPlan?> GetByIdAsync(WorkoutPlanId id, CancellationToken cancellationToken = default);

    Task<WorkoutPlan?> GetActiveByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default);

    void Remove(WorkoutPlan workoutPlan);

    void Update(WorkoutPlan workoutPlan);

    void UpdateRange(IEnumerable<WorkoutPlan> workoutPlans);
}
