using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseStats;

public sealed record GetExerciseStatsQuery(
    ExerciseId ExerciseId,
    ExerciseStatsRange Range,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null,
    ExerciseStatsChartMetric ChartMetric = ExerciseStatsChartMetric.Volume
) : IQuery<ExerciseStatsDto>;
