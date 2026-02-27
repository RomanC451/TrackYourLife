using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutFrequency;

public sealed record GetWorkoutFrequencyQuery(
    DateOnly? StartDate = null,
    DateOnly? EndDate = null,
    OverviewType OverviewType = OverviewType.Daily
) : IQuery<IReadOnlyList<WorkoutFrequencyDataDto>>;
