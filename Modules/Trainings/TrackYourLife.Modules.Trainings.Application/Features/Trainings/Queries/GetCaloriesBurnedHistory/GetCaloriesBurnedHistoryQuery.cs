using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetCaloriesBurnedHistory;

public sealed record GetCaloriesBurnedHistoryQuery(
    DateOnly? StartDate = null,
    DateOnly? EndDate = null,
    OverviewType OverviewType = OverviewType.Daily,
    AggregationType AggregationType = AggregationType.Sum
) : IQuery<IReadOnlyList<WorkoutAggregatedValueDto>>;
