using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans;

internal static class WorkoutPlanMappingsExtensions
{
    public static WorkoutPlanDto ToDto(this WorkoutPlanReadModel workoutPlan)
    {
        return new WorkoutPlanDto(
            Id: workoutPlan.Id,
            Name: workoutPlan.Name,
            IsActive: workoutPlan.IsActive,
            Workouts: workoutPlan.GetOrderedTrainings().Select(t => t.ToDto()).ToList(),
            CreatedOnUtc: workoutPlan.CreatedOnUtc,
            ModifiedOnUtc: workoutPlan.ModifiedOnUtc
        );
    }
}
