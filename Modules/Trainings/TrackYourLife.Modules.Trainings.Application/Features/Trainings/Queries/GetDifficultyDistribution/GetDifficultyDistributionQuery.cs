using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetDifficultyDistribution;

public sealed record GetDifficultyDistributionQuery(DateTime? StartDate, DateTime? EndDate)
    : IQuery<IReadOnlyList<DifficultyDistributionDto>>;
