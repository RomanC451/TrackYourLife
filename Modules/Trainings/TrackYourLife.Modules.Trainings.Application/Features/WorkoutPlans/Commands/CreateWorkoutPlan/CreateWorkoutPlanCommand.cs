using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.CreateWorkoutPlan;

public sealed record CreateWorkoutPlanCommand(string Name, bool IsActive, IReadOnlyList<TrainingId> TrainingIds)
    : ICommand<WorkoutPlanId>;
