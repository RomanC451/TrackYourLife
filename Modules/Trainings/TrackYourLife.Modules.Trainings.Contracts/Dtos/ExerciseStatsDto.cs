using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

public sealed record ExerciseStatsDto(
    ExerciseId ExerciseId,
    string ExerciseName,
    ExerciseStatsRange SelectedRange,
    DateOnly? WindowStartDate,
    DateOnly? WindowEndDate,
    ExerciseStatsChartMetric ChartMetric,
    bool IsSupportedExerciseType,
    bool HasEnoughData,
    ExerciseStatsSummaryDto Summary,
    IReadOnlyList<ExerciseImprovementTrendPointDto> ImprovementTrend,
    IReadOnlyList<ExerciseConsistencyPointDto> ConsistencyTrend
);
