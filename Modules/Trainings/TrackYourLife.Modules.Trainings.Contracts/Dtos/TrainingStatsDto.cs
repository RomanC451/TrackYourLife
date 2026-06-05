using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

public sealed record TrainingStatsDto(
    TrainingId TrainingId,
    string TrainingName,
    Difficulty Difficulty,
    IReadOnlyList<string> MuscleGroups,
    int ExerciseCount,
    int EstimatedDurationSeconds,
    ExerciseStatsRange SelectedRange,
    AggregationType ChartAggregationType,
    TrainingStatsSummaryDto Summary,
    IReadOnlyList<WorkoutAggregatedValueDto> DurationTrend,
    IReadOnlyList<WorkoutFrequencyDataDto> FrequencyTrend,
    IReadOnlyList<WorkoutAggregatedValueDto> CaloriesTrend,
    IReadOnlyList<WorkoutHistoryDto> RecentSessions
);
