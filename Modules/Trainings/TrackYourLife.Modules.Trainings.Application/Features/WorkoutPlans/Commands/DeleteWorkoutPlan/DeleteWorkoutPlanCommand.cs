using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.DeleteWorkoutPlan;

public sealed record DeleteWorkoutPlanCommand(WorkoutPlanId WorkoutPlanId) : ICommand;
