using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutHistory;

public sealed record GetWorkoutHistoryQuery(
    DateOnly? StartDate,
    DateOnly? EndDate
) : IQuery<IEnumerable<WorkoutHistoryDto>>;
