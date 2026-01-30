using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetCaloriesBurnedHistory;

public sealed record GetCaloriesBurnedHistoryQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    OverviewType OverviewType = OverviewType.Daily,
    AggregationType AggregationType = AggregationType.Sum
) : IQuery<IReadOnlyList<WorkoutAggregatedValueDto>>;
