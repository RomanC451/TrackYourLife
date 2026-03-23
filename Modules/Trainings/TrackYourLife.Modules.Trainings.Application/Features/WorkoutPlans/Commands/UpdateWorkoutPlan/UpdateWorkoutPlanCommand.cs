using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.UpdateWorkoutPlan;

public sealed record UpdateWorkoutPlanCommand(
    WorkoutPlanId WorkoutPlanId,
    string Name,
    bool IsActive,
    IReadOnlyList<TrainingId> TrainingIds
) : ICommand;
