using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetNextWorkoutFromActivePlan;

public sealed record GetNextWorkoutFromActivePlanQuery : IQuery<TrainingReadModel>;
