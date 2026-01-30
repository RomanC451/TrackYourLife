using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutFrequency;

public sealed record GetWorkoutFrequencyQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    OverviewType OverviewType = OverviewType.Daily
) : IQuery<IReadOnlyList<WorkoutFrequencyDataDto>>;
