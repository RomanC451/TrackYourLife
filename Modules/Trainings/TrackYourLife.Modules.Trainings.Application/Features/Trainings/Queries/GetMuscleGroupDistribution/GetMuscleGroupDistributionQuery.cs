using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetMuscleGroupDistribution;

public sealed record GetMuscleGroupDistributionQuery(DateTime? StartDate, DateTime? EndDate)
    : IQuery<IReadOnlyList<MuscleGroupDistributionDto>>;
