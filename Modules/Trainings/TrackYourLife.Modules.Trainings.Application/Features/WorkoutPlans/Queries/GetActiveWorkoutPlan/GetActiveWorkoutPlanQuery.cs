using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetActiveWorkoutPlan;

public sealed record GetActiveWorkoutPlanQuery : IQuery<WorkoutPlanReadModel>;
