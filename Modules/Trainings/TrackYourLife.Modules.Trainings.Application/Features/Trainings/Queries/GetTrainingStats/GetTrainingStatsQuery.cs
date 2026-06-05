using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingStats;

public sealed record GetTrainingStatsQuery(
    TrainingId TrainingId,
    ExerciseStatsRange Range,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null,
    AggregationType ChartAggregationType = AggregationType.Sum
) : IQuery<TrainingStatsDto>;
