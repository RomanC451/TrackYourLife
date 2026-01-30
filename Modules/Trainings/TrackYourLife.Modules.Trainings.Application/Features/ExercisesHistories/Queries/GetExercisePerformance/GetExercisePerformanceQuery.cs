using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExercisePerformance;

public sealed record GetExercisePerformanceQuery(
    DateTime? StartDate,
    DateTime? EndDate,
    ExerciseId? ExerciseId,
    PerformanceCalculationMethod CalculationMethod = PerformanceCalculationMethod.Sequential,
    int Page = 1,
    int PageSize = 10
) : IQuery<PagedList<ExercisePerformanceDto>>;
