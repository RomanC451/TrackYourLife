using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutStreak;

public sealed record GetWorkoutStreakQuery : IQuery<WorkoutStreakDto>;
