using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans;

internal sealed record WorkoutPlanDto(
    WorkoutPlanId Id,
    string Name,
    bool IsActive,
    IReadOnlyList<TrainingDto> Workouts,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
);
